using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectSlime : EnemyCore
{
    [Header("雷史莱姆的特效")]
    public GameObject UpHitAnim;
    public GameObject ElectHit;

    public AudioClip jumpAudio;
    public AudioClip downAudio;

    private float UpHitMulti = 2f;

    protected override void Awake_Core()
    {
        base.Awake_Core();
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        attachedElement.Add(ElementType.Electro, 1e9f);
        
        // 雷免疫
        getElementDamFuncList.Add(ElectroImmune);
        
        
        // 给特殊抓取器设置判断函数
        // 只有地面干员可以被计入
        SAG.operCheck = OC => Interpreter.isLow(InitManager.GetMap(OC.transform.position).type);
        // 敌人全部不计入
        SAG.enemyCheck = EC => false;
    }

    private float ElectroImmune(ElementSlot slot, float dam)
    {
        if (slot.eleType != ElementType.Electro) return dam;
        
        // 显示免疫文字
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = "免疫";
        text.color = new Color32(200, 200, 200, 255);
        text.transform.position = transform.position + new Vector3(0, 0.3f, 0);
        
        return 0;
    }
    

    protected override void Update_Core()
    {
        base.Update_Core();

        if (sp_.CanReleaseSkill() && operatorList_es.Count > 0)
        {
            anim.SetBool("sp", true);
        }
    }


    private Vector3 tarPos;
    private Vector3 prepos;
    public void SpAtkStart()
    {
        SpCannotMove();
        sp_.ReleaseSkill();
        
        // 找到范围内优先级最大的目标干员
        float maxPrio = -1e9f;
        OperatorCore tarOC = operatorList_es[0];
        foreach (var OC in operatorList_es)
        {
            if (OC.tarPriority > maxPrio)
            {
                maxPrio = OC.tarPriority;
                tarOC = OC;
            }
        }
        tarPos = tarOC.transform.position + new Vector3(0, 0, -0.01f);      // 目标位置
        prepos = tarPos;
        tarPos.y = 0;
        tarPos += anim.transform.localPosition;
        
        if (tarOC.transform.position.x - transform.position.x >= 0) ac_.TurnRight();
        else ac_.TurnLeft();
    }

    private Vector3 animLocalPos;
    public void SpAtkMove()
    {
        // 让本体远离，达到无法选中的效果
        animLocalPos = anim.transform.localPosition;
        anim.transform.SetParent(null);
        transform.position = new Vector3(999, 999, 999);
        StartCoroutine(SpMove());
    }

    IEnumerator SpMove()
    {
        float speed = 3f;
        while (Vector3.Distance(anim.transform.position, tarPos) > 0.05f)
        {// 移动到目标位置
            anim.transform.position = Vector3.Lerp(anim.transform.position, tarPos, speed * Time.deltaTime);
            yield return null;
        }
        anim.transform.position = tarPos;
    }

    public void SpAtkDown()
    {
        anim.transform.position = tarPos;
        anim.SetBool("sp", false);
        
        anim.transform.SetParent(transform);
        transform.position = prepos;
        anim.transform.localPosition = animLocalPos;
        epc_.ChangeRoute();
        
        ElementSlot elementSlot = new ElementSlot(ElementType.Electro, 2f);
        var tars = InitManager.GetNearByOper(prepos, 1.1f);
        foreach (var OC in tars)
        {
            Battle(OC, atk_.val * UpHitMulti, DamageMode.Physical, elementSlot,
                true, true);

            GameObject ehit = PoolManager.GetObj(ElectHit);
            ehit.transform.SetParent(OC.transform);
            ehit.transform.localPosition = new Vector3(0, 0, 0.35f);
            DurationRecycleObj recy = new DurationRecycleObj(ehit, 0.8f, OC, true);
            BuffManager.AddBuff(recy);
        }

        GameObject hitAnim = PoolManager.GetObj(UpHitAnim);
        hitAnim.transform.position = prepos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f);
        BuffManager.AddBuff(recycleObj);
    }

    public void SpAtkEnd()
    {
        SpCannotMoveEnd();
    }
    

    public override void OnAttack()
    {
        base.OnAttack();
        ElementSlot elementSlot = new ElementSlot(ElementType.Electro, 1f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, true);
    }

}
