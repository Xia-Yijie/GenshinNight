using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class wishUI : MonoBehaviour
{
    public static wishUI instance;
    
    public RectTransform selectObj;
    public Scrollbar contentBar;

    private void Awake()
    {
        instance = this;
    }

    /*
     * 0:无筛选界面
     * 1~7:风岩雷草水火冰，筛选界面
     */
    public List<GameObject> selectButtons = new List<GameObject>();
    public int showingID;
    public ElementType showingType;
    
    public void InitOpen()
    {// 最开始打开界面时，处于无筛选界面
        ChoosePage(0);
    }
    
    public void ChoosePage(int id)
    {
        if (id < 0 || id > 7) return;
        showingID = id;
        selectObj.SetParent(selectButtons[id].transform);
        selectObj.anchoredPosition = Vector2.zero;

        showingType = id switch
        {
            1 => ElementType.Anemo,
            2 => ElementType.Geo,
            3 => ElementType.Electro,
            4 => ElementType.Dendro,
            5 => ElementType.Hydro,
            6 => ElementType.Pyro,
            7 => ElementType.Cryo,
            _ => ElementType.None
        };

        RefreshPage(showingType);
        
        contentBar.value = 1;
    }

    public static void RefreshPage(ElementType selectElement)
    {
        instance.ClearSlots();
        
        // 先生成未解锁的角色
        for (int i = 0; i < gameManager.AllOperData.Count; i++)
        {
            var od = gameManager.AllOperData[i];
            if (selectElement != ElementType.None && od.elementType != selectElement) continue;
            if (gameManager.AllOperValid[od.Name]) continue;    // 如果已经激活，则放到后面
            instance.GetSlot(od, true);
        }
        // 再生成已解锁的角色
        for (int i = 0; i < gameManager.AllOperData.Count; i++)
        {
            var od = gameManager.AllOperData[i];
            if (selectElement != ElementType.None && od.elementType != selectElement) continue;
            if (gameManager.AllOperValid[od.Name]) instance.GetSlot(od, false);
        }

        instance.RefreshUISlot();
    }
    
    
    
    ///////////////////////// 对象池，用于存放角色祈愿条 /////////////////////////
    public GameObject ck_slot;
    public Transform slotPrt;
    public Transform disableSlotPrt;
    public List<RectTransform> eachRect = new List<RectTransform>();
    private List<wishSlot> poolValid = new List<wishSlot>();
    private List<wishSlot> poolUsed = new List<wishSlot>();
    
    public void GetSlot(operData od_, bool isAble)
    {// 生成一个条目，初始化并送到used中
        wishSlot slot;
        if (poolValid.Count > 0)
        {
            slot = poolValid[0];
            poolValid.RemoveAt(0);
        }
        else
        {
            slot = Instantiate(ck_slot, slotPrt).GetComponent<wishSlot>();
        }
        slot.gameObject.SetActive(true);
        slot.transform.SetParent(slotPrt);
        slot.Init(od_, isAble);
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
