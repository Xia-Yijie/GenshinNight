using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class parabola : MonoBehaviour
{
    private float g = 40f;                  // 重力加速度

    private BattleCore tarBattleCore;       // 追踪的目标
    private BattleCore attacker;            // 发射的来源
    private Transform tarTrans;             // 目标的transform
    private bool isNull;                    // 目标是否消失
    private float speed = 5f;               // 速度
    private float multi;                    // 本次攻击的倍率
    private Action<float, BattleCore, parabola, bool> reachFunc;    // 到达函数，如果attacker消失则不执行
    private Vector3 tarPY;                  // 目标初始偏移量
    private float durTime = 0;              // 整个运动需要的时间
    private Vector3 tarPos;                 // 目标的初始位置
    private Vector2 tarPos2D;
    private Vector2 direction2D;            // 在平面上移动的方向向量（归一化）
    private float VH;                       // 当前在垂直方向上的速度
    
    private Vector3 tarDetaPos;             // 目标移动的位置向量
    private Vector2 localPos;               // 如果目标未移动，当前应该处在的位置

    // private const float min_distance = 0.15f;
    // private float distance;
                     
    // private float py;

    public void Init(Vector3 pos, BattleCore attacker_, BattleCore targetBattleCore, float speed_ = 5,
        Action<float, BattleCore, parabola, bool> reach = null,
        float Multi = 1, Vector3 tarPY_ = default, float G = 40f)
    {
        tarBattleCore = targetBattleCore;
        tarTrans = tarBattleCore.animTransform;
        tarPos = tarTrans.position + tarPY_;
        if (targetBattleCore.dying)
        {
            Init(pos, attacker_, speed_, reach, Multi, tarPos, G);     // 执行无目标的初始化
            return;
        }

        norInit(pos, attacker_, speed_, reach, Multi, G);
        
        isNull = false;
        tarBattleCore.DieAction += TarNull;
    }

    public void Init(Vector3 pos, BattleCore attacker_, float speed_ = 5,
        Action<float, BattleCore, parabola, bool> reach = null, 
        float Multi = 1, Vector3 tarPos_ = default, float G = 40f)
    {// 没有目标的发射
        tarPos = tarPos_;
        norInit(pos, attacker_, speed_, reach, Multi, G);
        isNull = true;
    }

    private void norInit(Vector3 pos, BattleCore attacker_, float speed_,
        Action<float, BattleCore, parabola, bool> reach, float Multi, float G)
    {// 其他初始化，需要先初始化好目标点tarPos
        transform.position = pos;
        attacker = attacker_;
        speed = speed_;
        multi = Multi;
        reachFunc = reach;
        g = G;
        
        // 计算垂直初速度
        Vector2 prePos2D = BaseFunc.xz(pos);
        tarPos2D = BaseFunc.xz(tarPos);
        float distance = Vector2.Distance(prePos2D, tarPos2D);
        durTime = distance / speed;
        float detaH = pos.y - tarPos.y;
        VH = 0.5f * g * (durTime - (2 * detaH / g) / durTime);    // 初始垂直速度

        // 计算二维方向向量
        direction2D = tarPos2D - prePos2D;
        float k = Mathf.Sqrt(1f / (direction2D.x * direction2D.x + direction2D.y * direction2D.y));
        direction2D *= k;

        localPos = BaseFunc.xz(pos);
        tarDetaPos = Vector3.zero;
    }
    

    void Update()
    {
        // 计算剩余时间
        durTime -= Time.deltaTime;
        if (durTime <= 0)
        {// 时间到，已到达目标点
            Arrive();
            return;
        }
        
        // 计算二维平面平移
        localPos += direction2D * (speed * Time.deltaTime);
        
        // 计算垂直方向位移
        float disH = VH * Time.deltaTime;
        VH -= g * Time.deltaTime;
        
        if (!isNull)
        {// 如果目标还在，计算当前目标的偏移量
            tarDetaPos = tarTrans.position - tarPos;
        }
        
        // 根据移动量改变位置的xz，根据偏移量改变整体位置
        Vector3 pos = new Vector3(localPos.x, transform.position.y + disH, localPos.y);
        transform.position = pos + tarDetaPos;
    }

    private void Arrive()
    {
        if (!isNull)
            tarBattleCore.DieAction -= TarNull;

        if (attacker.gameObject.activeSelf)
            reachFunc?.Invoke(multi, isNull ? null : tarBattleCore, this, isNull);
        
        reachFunc = null;
        PoolManager.RecycleObj(gameObject);
    }
    
    private void TarNull(BattleCore bc_)
    {
        isNull = true;
    }
    
}