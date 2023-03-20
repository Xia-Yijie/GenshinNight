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
        int allOperCnt = gameManager.AllOperData.Count;
        int cnt = allOperCnt / 2 + allOperCnt % 2;

        for (int i = 0; i < cnt; i++)
        {
            GameObject obj = Instantiate(selectSlotsObj, selectPanel);
            selectSlotsList.Add(obj.transform);
        }

        for (int i = 0; i < allOperCnt; i++)
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

        for (int j = 0; j < operSelects.Count; j++)
        {
            operSelects[j].gameObject.SetActive(j < gameManager.UnlockCharacterNum);
        }
        int cnt = gameManager.UnlockCharacterNum / 2 + gameManager.UnlockCharacterNum % 2;
        for (int j = 0; j < selectSlotsList.Count; j++)
        {
            selectSlotsList[j].gameObject.SetActive(j < cnt);
        }
        
        
        int i = 0;
        foreach (var OD in gameManager.AllOperData)
        {
            if (!gameManager.AllOperValid[OD.EnName]) continue;
            if (gameManager.formation[gameManager.formationNum].Contains(OD))
            {
                operSelects[i].ChangeShowingOD(OD);
                operSelects[i].Selected();
                i++;
            }
        }

        foreach (var OD in gameManager.AllOperData)
        {
            if (!gameManager.AllOperValid[OD.EnName]) continue;
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
