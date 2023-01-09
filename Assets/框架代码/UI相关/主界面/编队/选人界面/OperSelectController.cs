using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperSelectController : MonoBehaviour
{
    public Transform selectPanel;
    public GameObject selectSlotsObj;
    public GameObject operSelectObj;
    private List<Transform> selectSlotsList = new List<Transform>();    // 长方形的选择容器，一个装俩select
    private List<FormationOperSelect> operSelects = new List<FormationOperSelect>();

    public List<operData> formationTmpList = new List<operData>();

    private FormationUIController fuc_;
    private FormationLeftUI flu_;
    public gradualChange thisPanel;

    private void Awake()
    {
        int cnt = gameManager.AllOperData.Count / 2 + gameManager.AllOperData.Count % 2;

        for (int i = 0; i < cnt; i++)
        {
            GameObject obj = Instantiate(selectSlotsObj, selectPanel);
            selectSlotsList.Add(obj.transform);
        }

        for (int i = 0; i < gameManager.AllOperData.Count; i++)
        {
            int k = i / 2;
            GameObject obj = Instantiate(operSelectObj, selectSlotsList[k]);
            operSelects.Add(obj.GetComponent<FormationOperSelect>());
        }

        // Open(null);
    }

    private void Start()
    {
        flu_ = MainSceneResource.instance.formationLeftUI;
        fuc_ = MainSceneResource.instance.formationUIController;
    }

    public void Open(operData od_)
    {
        flu_.ChangeOD(od_);
        MainSceneResource.instance.operSelectPanel.Show();
        
        int i = 0;
        foreach (var OD in gameManager.AllOperData)
        {
            if (gameManager.formation[gameManager.formationNum].Contains(OD))
            {
                operSelects[i].ChangeShowingOD(OD);
                operSelects[i].Selected();
                i++;
            }
        }

        foreach (var OD in gameManager.AllOperData)
        {
            if (!gameManager.formation[gameManager.formationNum].Contains(OD))
            {
                operSelects[i].ChangeShowingOD(OD);
                operSelects[i].UnSelected();
                i++;
            }
        }
    }

    public void ConfirmFormation()
    {
        int fnum = gameManager.formationNum;
        gameManager.formation[fnum].Clear();
        foreach (var i in formationTmpList)
        {
            gameManager.formation[fnum].Add(i);
        }
        fuc_.Refresh();
        formationTmpList.Clear();
        thisPanel.Hide();
    }
    
    public void DontConfirmFormation()
    {
        fuc_.Refresh();
        formationTmpList.Clear();
        thisPanel.Hide();
    }
    
    
}
