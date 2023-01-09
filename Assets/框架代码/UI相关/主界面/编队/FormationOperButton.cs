using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationOperButton : MonoBehaviour
{
    public FormationOperDisplay fod_;
    public operData od_;
    private OperSelectController osc_;

    private void Start()
    {
        osc_ = MainSceneResource.instance.operSelectController;
    }

    public void Press()
    {
        osc_.Open(od_);
    }

    /// <summary>
    /// 改变该展示框展示的干员
    /// </summary>
    public void ChangeShowingOper(operData nod_)
    {
        od_ = nod_;
        fod_.ChangeShowingOper(nod_);
    }
    


}
