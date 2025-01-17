using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PoolManager
{
    // 对象池
    private const int maxCount = 256;
    private static Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<string, GameObject> objPrt = new Dictionary<string, GameObject>();
    private static Transform PoolPrt;
    private static bool PoolPrtNotNull = false;
    
    public static void RecycleObj(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(objPrt[obj.name].transform);

        if (pool.ContainsKey(obj.name))
        {
            if (pool[obj.name].Count < maxCount)
            {
                pool[obj.name].Add(obj);
            }
            else Debug.Log("超过对象池最大上线");
        }
        else
        {
            pool.Add(obj.name, new List<GameObject>() { obj });
        }
    }
    
    public static GameObject GetObj(GameObject perfab)
    {
        // 如果没有父物体则生成
        if (!objPrt.ContainsKey(perfab.name))
        {
            if (!PoolPrtNotNull)
            {
                PoolPrt = new GameObject("对象池的特效们").transform;
                PoolPrtNotNull = true;
            }
            GameObject prt = new GameObject(perfab.name + "对象池");
            prt.transform.SetParent(PoolPrt);
            objPrt.Add(perfab.name, prt);
        }

        // 池子中有
        GameObject result = null;
        if (pool.ContainsKey(perfab.name))
        {
            if (pool[perfab.name].Count > 0)
            {
                result = pool[perfab.name][0];
                result.SetActive(true);
                pool[perfab.name].Remove(result);
                result.transform.SetParent(objPrt[perfab.name].transform);
                return result;
            }
        }
        // 池子中缺少
        result = Object.Instantiate(perfab, null);
        result.name = perfab.name;
        RecycleObj(result);
        GetObj(result);
        result.transform.SetParent(objPrt[perfab.name].transform);
        return result;
    }

    /// <summary>
    /// 根据名称找obj，如果没有返回一个同名的空物体
    /// </summary>
    public static GameObject GetObj(string Name)
    {
        // 如果没有父物体则生成
        if (!objPrt.ContainsKey(Name))
        {
            objPrt.Add(Name, new GameObject(Name + "对象池"));
        }

        // 池子中有
        GameObject result = null;
        if (pool.ContainsKey(Name))
        {
            if (pool[Name].Count > 0)
            {
                result = pool[Name][0];
                result.SetActive(true);
                pool[Name].Remove(result);
                result.transform.SetParent(objPrt[Name].transform);
                return result;
            }
        }
        // 池子中缺少
        result = new GameObject(Name);
        RecycleObj(result);
        GetObj(result);
        result.transform.SetParent(objPrt[Name].transform);
        return result;
    }
    
    public static void Clear()
    {
        pool.Clear();
        objPrt.Clear();
        EnemyPoolManager.Clear();
    }
    
}

public abstract class PrtRecycleObj : BuffSlot
{
    protected BattleCore bc_;
    protected GameObject obj;
    protected bool isDie;
    
    public PrtRecycleObj(GameObject object_,BattleCore Prt)
    {
        obj = object_;
        bc_ = Prt;

        if (bc_.dying)
        {
            Die(bc_);
        }
        else bc_.DieAction += Die;
    }

    public override bool BuffEndCondition()
    {
        return isDie;
    }
    
    public override void BuffEnd()
    {
        if (!isDie)
        {
            bc_.DieAction -= Die;
        }
        PoolManager.RecycleObj(obj);
    }

    private void Die(BattleCore battleCore)
    {
        isDie = true;
    }
}

public class SkillRecycleObj : PrtRecycleObj
{
    private SPController sp_;

    public SkillRecycleObj(GameObject object_, BattleCore battleCore) : base(object_, battleCore)
    {
        sp_ = bc_.sp_;
    }

    public override void BuffStart() { }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        return !sp_.during || base.BuffEndCondition();
    }
}