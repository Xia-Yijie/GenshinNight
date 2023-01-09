using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationOperDisplay : MonoBehaviour
{
    public operData od_;
    
    public GameObject empty;
    public GameObject role;
    public Image operImage;
    public Text Name;
    public Image elementImage;
    public GameObject[] roleBackground = new GameObject[6];     // 1-6星干员背景
    public GameObject[] roleInfoBackground = new GameObject[6]; // 1-6星干员背景

    public Image[] skillBackGround = new Image[3];              // 干员技能的背景颜色
    public Image[] skillImages = new Image[3];                  // 干员的3个技能
    // public GameObject[] elitismImage = new GameObject[2];
    // public Text levelText;
    
    private bool isEmpty = true;

    /// <summary>
    /// 改变该展示框展示的干员
    /// </summary>
    public void ChangeShowingOper(operData nod_)
    {
        if (nod_ == null)
        {
            isEmpty = true;
            role.SetActive(false);
            empty.SetActive(true);
            return;
        }
        
        isEmpty = false; 
        role.SetActive(true);
        empty.SetActive(false);
        
        od_ = nod_;
        operImage.sprite = od_.illustratedBookImage;
        Name.text = od_.Name;
        elementImage.sprite = StoreHouse.GetElementSprite(od_.elementType);
        
        for (int i = 0; i < 6; i++)
        {
            if (i == (int) od_.star)
            {
                roleBackground[i].SetActive(true);
                roleInfoBackground[i].SetActive(true);
            }
            else
            {
                roleBackground[i].SetActive(false);
                roleInfoBackground[i].SetActive(false);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (od_.elitismCost[i] > 1e5) skillBackGround[i + 1].gameObject.SetActive(false);
            else skillBackGround[i + 1].gameObject.SetActive(true);
        }
        for (int i = 0; i < 3; i++)
        {
            skillBackGround[i].color = StoreHouse.GetElementDamageColor(od_.elementType);
            skillImages[i].sprite = od_.skillImage[i];
        }
    }
    
}
