using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainLevelSelect : MonoBehaviour
{
    private static levelData showingLD;

    private void Start()
    {
        
    }
    
    public static void Hide_MainLevelSelectPanel()
    {
        MainSceneResource.instance.mainLevelSelectPanel.Hide();
    }

    public static void Show_mls_rightDetailPanel(levelData ld_)
    {
        showingLD = ld_;
        MainSceneResource.instance.mls_rightDetailPanel.Show();
        
        MainSceneResource.instance.mlsr_IDText.text = ld_.ID_show;
        MainSceneResource.instance.mlsr_nameText.text = ld_.Name_show;
        MainSceneResource.instance.mlsr_descriptionText.text = ld_.Description;
        MainSceneResource.instance.mlsr_highLightDescriptionText.text = ld_.HighLightDescription;
        MainSceneResource.instance.mlsr_recommendLevel.text = ld_.recommendLevel;
    }

    public static void Hide_mls_rightDetailPanel()
    {
        MainSceneResource.instance.mls_rightDetailPanel.Hide();
    }

    public void ShowSelectPanel()
    {
        MainSceneResource.instance.formationUIController.Open(showingLD);
    }
    
}
