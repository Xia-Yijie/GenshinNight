using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yelan : OperatorCore
{
    private float[] Skill1_Multi = {1.8f, 2.1f, 2.4f, 2.8f, 3.2f, 3.7f, 4.2f};
    private float[] Skill2_Multi = {0.6f, 0.7f, 0.85f, 1.05f, 1.3f, 1.65f, 2f};
    private float[] Skill2_Duration = {15f, 16f, 17f, 18f, 19f, 21f, 23f, 25f};
    private float Skill2_SlowRate = 0.8f;
    private float Skill2_SlowDuration = 1f;
    private float[] Skill3_Multi = {0.35f, 0.41f, 0.48f, 0.56f, 0.64f, 0.73f, 0.84f};
    private float[] Skill3_Duration = {13f, 13f, 14f, 15f, 16f, 17f, 18f};
    private float Skill3_DamIncInit = 0.01f;
    private float Skill3_DamIncPerSecend = 0.035f;

    private float[] talent1_LifeIncRate = {0.06f, 0.12f, 0.18f, 0.25f};
    private float talent2_IncRate = 0.12f;
    
    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "夜兰的下一次攻击将发射“破局矢”，造成" +
                       CT.ChangeToColorfulPercentage(Skill1_Multi[lel]) +
                       "的范围" +
                       CT.GetColorfulText("水元素物理", CT.HydroBlue) +
                       "伤害";
            case 1:
                return "夜兰在正前方部署3道「" +
                       CT.GetColorfulText("络命丝", CT.HydroBlue) +
                       "」，持续" +
                       CT.GetColorfulText(Skill2_Duration[lel].ToString("f0")) +
                       "秒\n\n敌人在经过一道" +
                       CT.GetColorfulText("络命丝", CT.HydroBlue) +
                       "时，会受到一次" +
                       CT.ChangeToColorfulPercentage(Skill2_Multi[lel]) +
                       "的" + CT.GetColorfulText("水元素物理", CT.HydroBlue) +
                       "伤害，并被减速" +
                       CT.ChangeToColorfulPercentage(Skill2_SlowRate) +
                       "，持续" +
                       CT.GetColorfulText(Skill2_SlowDuration.ToString("f0")) +
                       "秒";
            default:
                return "夜兰选取范围内最近（优先选取正前方）的一名友方干员，为其凝聚「" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "」协助战斗，" + CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将持续" +
                       CT.GetColorfulText(Skill3_Duration[lel].ToString("f0")) +
                       "秒：\n\n·当附属干员进行普通攻击时，" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将进行协同攻击，造成3次" +
                       CT.ChangeToColorfulPercentage(Skill3_Multi[lel]) + "的" +
                       CT.GetColorfulText("水元素物理", CT.HydroBlue) + "伤害，该效果有" +
                       CT.GetColorfulText("2", CT.normalRed) + "秒冷却\n\n" +
                       "·为附属干员提供" +
                       CT.ChangeToColorfulPercentage(Skill3_DamIncInit) +
                       "的元素伤害加成，且每过一秒，该效果还将提高" +
                       CT.ChangeToColorfulPercentage(Skill3_DamIncPerSecend, 1) + "\n\n·" +
                       CT.GetColorfulText("玄掷玲珑", CT.HydroBlue) +
                       "将在夜兰退场时消失";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            string s = "";
            for (int i = 0; i < 4; i++)
            {
                s += CT.ChangeToColorfulPercentage(talent1_LifeIncRate[i]);
                if (i < 3) s += "/";
            }

            return "部署时根据场上干员的元素类型，每有1/2/3/4种不同的类型，夜兰获得" +
                   s + "的生命值上限提升";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "夜兰造成的所有" +
                   CT.GetColorfulText("水元素", CT.HydroBlue) +
                   "伤害得到提升，提升值相当于夜兰当前生命值的" +
                   CT.ChangeToColorfulPercentage(talent2_IncRate);
        }
    }
}
