using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canKnowSettingUI : MonoBehaviour
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
    public List<RectTransform> eachRect = new List<RectTransform>();
    public Transform disableSlotPrt;
    public AudioClip enableAllAudio;
    public AudioClip disableAllAudio;
    
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
        
        if (!isStrengthen) generater[id].GenerateUniversalKnowledge(false);
        else generater[id].GenerateUniversalKnowledge_S(false);


        RedBackGround.SetActive(isStrengthen);
        strengthenImage.sprite = isStrengthen ? StrengthenSprite : CommonSprite;
        strengthenText.text = isStrengthen ? "强化" : "普通";
        contentBar.value = 1;
    }
    
    public void ClickStrengthen()
    {
        isStrengthen = !isStrengthen;
        RefreshPage();
    }
    
    public void PlayStrengthenAudio()
    {
        AudioManager.PlayEFF(isStrengthen
            ? ShopUIController.instance.SelectStrengthenAudio
            : ShopUIController.instance.CancelStrengthenAudio, 0.6f);
    }

    public void RefreshPage()
    {// 刷新页面
        ChoosePage(showingID);
    }

    public void EnableAll()
    {
        ShopUIController.ShowText("所有的罐装知识都激活了，有没有感觉到力量在身体内流淌呢？");
        gameManager.knowledgeData.EnableAll();
        gameManager.knowledgeDataStrengthen.EnableAll();
        AudioManager.PlayEFF(enableAllAudio);
        RefreshPage();
    }

    public void DisAbleAll()
    {
        ShopUIController.ShowText("所有的罐装知识都被关闭了，是想要挑战自己吗？");
        gameManager.knowledgeData.DisableAll();
        gameManager.knowledgeDataStrengthen.DisableAll();
        AudioManager.PlayEFF(disableAllAudio);
        RefreshPage();
    }


    ///////////////////////// 对象池，用于存放罐装知识配置 /////////////////////////
    public GameObject ck_slot;
    public Transform slotPrt;
    private List<canKnowSettingSlot> poolValid = new List<canKnowSettingSlot>();
    private List<canKnowSettingSlot> poolUsed = new List<canKnowSettingSlot>();
    
    public void GetSlot(Sprite sprite, string title, string description, 
        KnowledgeBuffer buffer)
    {// 生成一个条目，初始化并送到used中
        canKnowSettingSlot slot;
        if (poolValid.Count > 0)
        {
            slot = poolValid[0];
            poolValid.RemoveAt(0);
        }
        else
        {
            slot = Instantiate(ck_slot, slotPrt).GetComponent<canKnowSettingSlot>();
        }
        slot.gameObject.SetActive(true);
        slot.transform.SetParent(slotPrt);
        slot.Init(sprite, title, description, buffer);
        poolUsed.Add(slot);
    }
    
    public void RefreshUISlot()
    {// 刷新上层UI，避免出现扩展不成功的现象
        foreach (var i in eachRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(i);
        }
    }
    
    public void ClearSlots()
    {// 清空所有的slot
        foreach (var i in poolUsed)
        {
            i.gameObject.SetActive(false);
            i.transform.SetParent(disableSlotPrt);
            poolValid.Add(i);
        }
        poolUsed.Clear();
    }
}
