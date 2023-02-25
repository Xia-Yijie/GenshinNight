using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FormationUIController : MonoBehaviour
{
    public gradualChange formationUI;

    public List<FormationOperButton> operButtonList = new List<FormationOperButton>();
    public GameObject SupportRole;
    public GameObject StartButton;
    public RectTransform RoleListPanel;

    public GameObject[] TeamSelected = new GameObject[4];
    public GameObject[] TeamUnselected = new GameObject[4];

    public levelData ld_;

    public void Open()
    {// 给按钮用的函数
        Open(null);
    }
    
    public void Open(levelData LD)
    {
        formationUI.Show();
        ld_ = LD;
        // 如果是在初始界面进入的编队，没有右边的助战和开始，且干员列表居中
        if (LD != null)  
        {
            SupportRole.SetActive(true);
            StartButton.SetActive(true);
            RoleListPanel.anchoredPosition = new Vector2(-118, 3);
        }
        else
        {
            SupportRole.SetActive(false);
            StartButton.SetActive(false);
            RoleListPanel.anchoredPosition = new Vector2(0, 3);
        }
        Refresh();
    }

    public void Refresh()
    {
        int fnum = gameManager.formationNum;
        for (int i = 0; i < 12; i++)
        {
            if (i < gameManager.formation[fnum].Count)
                operButtonList[i].ChangeShowingOper(gameManager.formation[fnum][i]);
            else operButtonList[i].ChangeShowingOper(null);
        }
    }

    public void ChangeFormationNum(int num)
    {
        gameManager.formationNum = num;
        Refresh();
        for (int i = 0; i < 4; i++)
        {
            if (i == num)
            {
                TeamSelected[i].SetActive(true);
                TeamUnselected[i].SetActive(false);
            }
            else
            {
                TeamSelected[i].SetActive(false);
                TeamUnselected[i].SetActive(true);
            }
        }
    }

    public void LeftFormationButton()
    {
        int num = gameManager.formationNum - 1 < 0 ? 3 : gameManager.formationNum - 1;
        ChangeFormationNum(num);
    }

    public void RightFormationButton()
    {
        int num = gameManager.formationNum + 1 > 3 ? 0 : gameManager.formationNum + 1;
        ChangeFormationNum(num);
    }

    public void StartGame()
    {
        InitManager.ld_ = ld_;
        InitManager.globalArgs = (GlobalLevelArgs) ld_.args.Clone();    // 将参数浅拷贝到InitManager中
        SceneSwitch.LoadScene(ld_.Name_set, ld_);
    }

}
