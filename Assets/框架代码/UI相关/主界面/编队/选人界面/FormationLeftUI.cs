using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationLeftUI : MonoBehaviour
{
    public operData od_;
    public GameObject noInfoObj;
    public GameObject InfoObj;
    public List<RectTransform> eachRect = new List<RectTransform>();
    
    public Text NameText;
    public Button illustrationButton;
    public Image elementImage;
    public Text deploymentTypeText;
    public Image atkRangeImage;
    public Text reTimeText;
    public Text costText;
    public Text atkSpeedText;
    public Text blockText;
    public Text lifeText;
    public Text atkText;
    public Text defText;
    public Text magicDefText;
    // public Text levelText;
    // public Text maxLevelText;

    public GameObject[] skillObj = new GameObject[3];
    public Text[] skillName = new Text[3];
    public Image[] skillImage = new Image[3];
    public Text[] initSpText = new Text[3];
    public Text[] maxSpText = new Text[3];
    public Image[] recoverTypeImage = new Image[3];
    public Text[] recoverTypeText = new Text[3];
    public Text[] releaseTypeText = new Text[3];
    public Text[] durationText = new Text[3];
    public Text[] descriptionText = new Text[3];

    private void Start()
    {
        ChangeOD(od_);
    }

    public void ChangeOD(operData newOD)
    {
        od_ = newOD;
        if (od_ == null)
        {
            noInfoObj.SetActive(true);
            InfoObj.SetActive(false);
            return;
        }
        noInfoObj.SetActive(false);
        InfoObj.SetActive(true);
        
        //基础属性
        NameText.text = od_.Name;
        elementImage.sprite = StoreHouse.GetElementSprite(od_.elementType);
        deploymentTypeText.text = GetDeploymentTypeText(od_);
        atkRangeImage.sprite = od_.atkRangeImage[0];
        reTimeText.text = GetSpeedDescriptionText(od_.resetSpeedDescription);
        costText.text = od_.cost.ToString();
        atkSpeedText.text = GetSpeedDescriptionText(od_.atkSpeedDescription);
        blockText.text = od_.maxBlock.ToString();
        lifeText.text = od_.life.ToString("f0");
        atkText.text = od_.atk.ToString("f0");
        defText.text = od_.def.ToString("f0");
        magicDefText.text = od_.magicDef.ToString("f0");
        
        //技能部分
        for (int i = 0; i < 2; i++)
        {// 如果当次精英化花费过大，则认为没有下一个技能
            skillObj[i + 1].SetActive(od_.elitismCost[i] < 1e5);
        }

        skillName[0].text = od_.skillName0;
        skillName[1].text = od_.skillName1;
        skillName[2].text = od_.skillName2;
        for (int i = 0; i < 3; i++) skillImage[i].sprite = od_.skillImage[i];
        initSpText[0].text = od_.initSP0[0].ToString();
        initSpText[1].text = od_.initSP1[0].ToString();
        initSpText[2].text = od_.initSP2[0].ToString();
        maxSpText[0].text = od_.maxSP0[0].ToString();
        maxSpText[1].text = od_.maxSP1[0].ToString();
        maxSpText[2].text = od_.maxSP2[0].ToString();
        
        recoverTypeImage[0].color = SkillUIController.GetRecoverTypeColor(od_.skill0_recoverType);
        recoverTypeImage[1].color = SkillUIController.GetRecoverTypeColor(od_.skill1_recoverType);
        recoverTypeImage[2].color = SkillUIController.GetRecoverTypeColor(od_.skill2_recoverType);
        recoverTypeText[0].text = SkillUIController.GetRecoverTypeText(od_.skill0_recoverType);
        recoverTypeText[1].text = SkillUIController.GetRecoverTypeText(od_.skill1_recoverType);
        recoverTypeText[2].text = SkillUIController.GetRecoverTypeText(od_.skill2_recoverType);
        releaseTypeText[0].text = SkillUIController.GetReleaseTypeText(od_.skill0_releaseType);
        releaseTypeText[1].text = SkillUIController.GetReleaseTypeText(od_.skill1_releaseType);
        releaseTypeText[2].text = SkillUIController.GetReleaseTypeText(od_.skill2_releaseType);
        durationText[0].text = od_.duration0[0].ToString("f0");
        durationText[1].text = od_.duration1[0].ToString("f0");
        durationText[2].text = od_.duration2[0].ToString("f0");
        
        descriptionText[0].text = FillDescriptionText(od_.description0[0]);
        descriptionText[1].text = FillDescriptionText(od_.description1[0]);
        descriptionText[2].text = FillDescriptionText(od_.description2[0]);

        foreach (var i in eachRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(i);
        }
    }

    public static string GetDeploymentTypeText(operData od)
    {
        return od.banHighGround switch
        {
            false when !od.banLowGround => "通用干员",
            true when !od.banLowGround => "地面干员",
            false when od.banLowGround => "高台干员",
            _ => "错误干员"
        };
    }
    
    public static string GetSpeedDescriptionText(speedDescription sd)
    {
        return sd switch
        {
            speedDescription.veryFast => "非常快",
            speedDescription.littleFast => "较快",
            speedDescription.fast => "快",
            speedDescription.normal => "普通",
            speedDescription.slow => "慢",
            speedDescription.littleSlow => "较慢",
            speedDescription.verySlow => "非常慢",
            _ => "错误"
        };
    }

    private string FillDescriptionText(string s)
    {
        if (s.Length >= 55) return s;
        int num = 0;
        foreach (var i in s)
        {
            if (i == '\n') num++;
        }
        if (num >= 2) return s;
        for (int i = num; i < 2; i++) s = s + "\n";
        return s;
    }
    
}
