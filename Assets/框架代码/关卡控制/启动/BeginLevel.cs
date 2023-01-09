using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginLevel : MonoBehaviour
{
    private void Awake()
    {
        if (gameManager.formation[gameManager.formationNum] == null) return;
        foreach (var i in gameManager.formation[gameManager.formationNum])
        {
            InitManager.Register(i, 1);
        }
    }
    
    void Start()
    {
        InitManager.Init();
        OperUIManager.Init();
        BuffManager.Init();
    }
    
}
