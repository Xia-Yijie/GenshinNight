using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canningKnowledgeUI : MonoBehaviour
{
    public ShopUIController shop_;
    public List<baseKnowledgeGenerator> generater = new List<baseKnowledgeGenerator>();
    public RectTransform selectObj;
    public GameObject RedBackGround;
    public Image strengthenImage;
    public Text strengthenText;
    public Sprite CommonSprite;
    public Sprite StrengthenSprite;
    public Scrollbar contentBar;

    /*
     * 0:通用罐装知识界面
     * 1~7:风岩雷草水火冰，元素罐装知识界面
     */
    public List<GameObject> selectButtons = new List<GameObject>();
    public int showingID;
    public bool isStrengthen = false;


    public void InitOpen()
    {// 最开始打开界面时，处于通用罐装知识界面
        if(isStrengthen) ClickStrengthen();
        ChoosePage(0);
    }

    public void ChoosePage(int id)
    {
        if (id < 0 || id > 7) return;
        showingID = id;
        selectObj.SetParent(selectButtons[id].transform);
        selectObj.anchoredPosition = Vector2.zero;
        
        if (!isStrengthen) generater[id].GenerateUniversalKnowledge(true);
        else generater[id].GenerateUniversalKnowledge_S(true);

        RedBackGround.SetActive(isStrengthen);
        strengthenImage.sprite = isStrengthen ? StrengthenSprite : CommonSprite;
        strengthenText.text = isStrengthen ? "强化罐装知识" : "普通罐装知识";
        contentBar.value = 1;
    }

    public void ClickStrengthen()
    {
        isStrengthen = !isStrengthen;
        RefreshPage();
    }

    public void RefreshPage()
    {// 刷新页面
        ChoosePage(showingID);
    }
    
    
    
    
    
    ///////////////////////// 对象池，用于存放罐装知识条目 /////////////////////////
    public GameObject ck_slot;
    public Transform slotPrt;
    private List<canningKnowledgeSlot> poolValid = new List<canningKnowledgeSlot>();
    private List<canningKnowledgeSlot> poolUsed = new List<canningKnowledgeSlot>();
    
    public void GetSlot(Sprite sprite, string title, string description, int price, 
        Action buyAction, int remainNum, int totalNum)
    {// 生成一个条目，初始化并送到used中
        canningKnowledgeSlot slot;
        if (poolValid.Count > 0)
        {
            slot = poolValid[0];
            poolValid.RemoveAt(0);
        }
        else
        {
            slot = Instantiate(ck_slot, slotPrt).GetComponent<canningKnowledgeSlot>();
        }
        slot.gameObject.SetActive(true);
        slot.Init(sprite, title, description, price, buyAction, remainNum, totalNum);
        poolUsed.Add(slot);
    }

    public void ClearSlots()
    {// 清空所有的slot
        foreach (var i in poolUsed)
        {
            i.gameObject.SetActive(false);
            poolValid.Add(i);
        }
        poolUsed.Clear();
    }
}
