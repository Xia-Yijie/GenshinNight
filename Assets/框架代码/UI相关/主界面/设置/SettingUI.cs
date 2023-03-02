using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    public static SettingUI instance;
    public gradualChange selfGra;
    public SaveUIController saveUIController;

    public gradualChange confirmPage;
    public Text confirmPageText;
    public Action confirmPage_YES;

    public gradualChange informationPage;
    public Text informationPageText;


    private void Awake()
    {
        instance = this;
        selfGra = GetComponent<gradualChange>();
    }

    public void Open()
    {
        selfGra.Show();
        SaveUIController.Refresh();
    }

    public void Close()
    {
        selfGra.Hide();
    }

    public static void ShowConfirmPage(string text, Action YES_Action)
    {
        instance.confirmPage.Show();
        instance.confirmPageText.text = text;
        instance.confirmPage_YES = YES_Action;
    }

    public static void ShowInformationPage(string text)
    {
        instance.informationPage.Show();
        instance.informationPageText.text = text;
    }

    public void ConfirmPage_ClickYes()
    {
        confirmPage_YES?.Invoke();
        confirmPage.Hide();
    }
    
    public void ConfirmPage_ClickNo()
    {
        confirmPage.Hide();
    }
    
    public void InformationPage_ClickExit()
    {
        informationPage.Hide();
    }
    
}
