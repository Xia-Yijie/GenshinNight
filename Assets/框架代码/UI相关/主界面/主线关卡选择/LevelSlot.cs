using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSlot : MonoBehaviour
{
    public levelData ld_;


    public void SelectThis()
    {
        mainLevelSelect.Show_mls_rightDetailPanel(ld_);
    }
    
    
}
