using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FormationOperSelect : MonoBehaviour
{
    public GameObject blueSelectObj;
    public FormationOperDisplay fod_;
    public bool isSelect = false;

    private operData od_;
    private OperSelectController osc_;
    private FormationLeftUI flui_;
    

    private void Start()
    {
        osc_ = MainSceneResource.instance.operSelectController;
        flui_ = MainSceneResource.instance.formationLeftUI;
    }

    public void Press()
    {
        flui_.ChangeOD(od_);
        if (isSelect)
        {
            UnSelected();
        }
        else
        {
            Selected();
        }
    }

    public void Selected()
    {
        if (od_ == null) return;
        isSelect = true;
        blueSelectObj.SetActive(true);
        if (!osc_.formationTmpList.Contains(od_)) osc_.formationTmpList.Add(od_);
    }

    public void UnSelected()
    {
        if (od_ == null) return;
        isSelect = false;
        blueSelectObj.SetActive(false);
        if (osc_.formationTmpList.Contains(od_)) osc_.formationTmpList.Remove(od_);
    }

    public void ChangeShowingOD(operData od)
    {
        od_ = od;
        fod_.ChangeShowingOper(od);
    }
    

}
