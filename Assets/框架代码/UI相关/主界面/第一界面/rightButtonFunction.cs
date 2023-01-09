using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightButtonFunction : MonoBehaviour
{
    private gradualChange mainLevelSelectPanel;

    private void Start()
    {
        mainLevelSelectPanel = MainSceneResource.instance.mainLevelSelectPanel;
    }

    public void ShowMainLevelSelectPanel()
    {
        mainLevelSelectPanel.Show();
    }
    
}
