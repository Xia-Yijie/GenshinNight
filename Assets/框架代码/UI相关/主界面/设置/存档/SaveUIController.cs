using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveUIController : MonoBehaviour
{
    public static SaveUIController instance;
    public List<SaveSlot> saveSlots = new List<SaveSlot>();

    private void Awake()
    {
        instance = this;
    }

    public static void Refresh()
    {
        foreach (var slot in instance.saveSlots)
        {
            slot.Init();
        }
    }


    public static void Save(int id)
    {
        SaveManager.Save(id);
    }
}
