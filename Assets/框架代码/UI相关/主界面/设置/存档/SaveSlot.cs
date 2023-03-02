using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public int id;
    
    public GameObject ExistObj;
    public GameObject EmptyObj;
    public Text PrimogemText;
    public Text MoraText;

    private bool dataExist;

    public void Start()
    {
        SaveUIController.instance.saveSlots.Add(this);  // 向当前的controller注册
    }

    public void Init()
    {
        var data = SaveManager.Load_Small(id);
        if (data == null)
        {
            ExistObj.SetActive(false);
            EmptyObj.SetActive(true);
            dataExist = false;
            return;
        }
        ExistObj.SetActive(true);
        EmptyObj.SetActive(false);
        dataExist = true;

        PrimogemText.text = data.Primogem.ToString();
        MoraText.text = data.Mora.ToString();
    }

    public void ClickSave()
    {
        if (!dataExist)
        {
            ActuallySave();
            return;
        }

        SettingUI.ShowConfirmPage("确定将数据保存在这里吗？原来的存档将会被覆盖", ActuallySave);
    }

    public void ClickLoad()
    {
        if (!dataExist)
        {
            SettingUI.ShowInformationPage("这里还没有保存过数据哦");
            return;
        }

        SettingUI.ShowConfirmPage("确定要读取此存档吗（未保存的数据将会消失）", ActuallyLoad);
    }

    private void ActuallySave()
    {
        SaveManager.Save(id);
        SettingUI.ShowInformationPage("数据保存成功！");
        SaveUIController.Refresh();
    }

    private void ActuallyLoad()
    {
        SaveManager.Load(id);
        SettingUI.ShowInformationPage("数据读取成功！");
    }
    

}
