using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneResource : MonoBehaviour
{
    public static MainSceneResource instance;
    
    public gradualChange mainLevelSelectPanel;
    public gradualChange mls_rightDetailPanel;
    public gradualChange formationPanel;
    public gradualChange operSelectPanel;
    
    
    public Text mlsr_nameText;
    public Text mlsr_IDText;
    public Text mlsr_descriptionText;
    public Text mlsr_highLightDescriptionText;
    public Text mlsr_recommendLevel;
    public Image mlsr_mapImage;

    public OperSelectController operSelectController;
    public FormationUIController formationUIController;
    public FormationLeftUI formationLeftUI;
    
    
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            instance = this;
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
}
