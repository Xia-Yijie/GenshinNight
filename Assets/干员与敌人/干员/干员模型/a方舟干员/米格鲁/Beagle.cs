using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beagle : OperatorCore
{
    private float[] DefIncrease = {0.1f, 0.15f, 0.2f, 0.3f, 0.35f, 0.4f, 0.5f};
    private float[] GeoAtkMulti = {1.2f, 1.25f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f};

    public override void SkillStart_1()
    {
        float defInc = DefIncrease[skillLevel[0]];
        SkillValueBuff valueBuff = new SkillValueBuff(def_, ValueBuffMode.Percentage, defInc, this);
        BuffManager.AddBuff(valueBuff);

        GameObject light = PoolManager.GetObj(StoreHouse.instance.underGroundLight);
        Vector3 pos = new Vector3(0, 0, -0.3f);
        light.transform.SetParent(transform);
        light.transform.localPosition = pos;
        SkillRecycleObj recycleObj = new SkillRecycleObj(light, this);
        BuffManager.AddBuff(recycleObj);
    }
    
    public override void SkillStart_2()
    {
        GameObject light = PoolManager.GetObj(StoreHouse.instance.underGroundLight);
        Vector3 pos = new Vector3(0, 0, -0.3f);
        light.transform.SetParent(transform);
        light.transform.localPosition = pos;
        SkillRecycleObj recycleObj = new SkillRecycleObj(light, this);
        BuffManager.AddBuff(recycleObj);
    }

    public override void OnAttack()
    {
        if (skillNum == 1 && sp_.during)
        {
            GeoAttack();
        }
        else
        {
            Battle(target, atk_.val, DamageMode.Physical);
        }
    }

    private void GeoAttack()
    {
        ElementSlot elementSlot = new ElementSlot(ElementType.Geo, 1f);
        float multi = GeoAtkMulti[skillLevel[1]];
        Battle(target, atk_.val * multi, DamageMode.Physical, 
            elementSlot, defaultElementTimer, true);
    }


    public override string GetSkillDescription(int SkillID)
    {
        if (SkillID == 0)
        {
            return "防御力+" + 
                   CT.ChangeToColorfulPercentage(DefIncrease[skillLevel[0]], CT.normalBlue);
        }
        else
        {
            return "米格鲁让手中的剑附着岩元素的力量，在技能持续时间内，米格鲁的攻击将转换为" +
                   CT.GetColorfulText("岩元素伤害", CT.GeoYellow) +
                   "并且造成攻击力" +
                   CT.ChangeToColorfulPercentage(GeoAtkMulti[skillLevel[0]], CT.normalBlue) +
                   "的伤害\n" + "" +
                   CT.GetColorfulText("1", CT.normalBlue) +
                   "岩元素附着量，" +
                   CT.GetColorfulText("3", CT.normalBlue)
                   + "秒独立元素附着计时器\n";
        }
    }
    
    
    
    
}
