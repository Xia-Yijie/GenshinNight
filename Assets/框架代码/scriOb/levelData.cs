using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New levelData",menuName = "myScript/levelData")]
public class levelData : ScriptableObject
{
    public string Name_set;
    public string Name_show;
    public string ID_show;
    public string recommendLevel;
    
    [TextArea] 
    public string Description;
    
    [TextArea] 
    public string HighLightDescription;

    [Header("地图快照")] 
    public Sprite map;

    [Header("加载背景")] 
    public Sprite loadBackground;
    
    [Header("背景音乐")] 
    public AudioClip BGM;

    [Header("初始数据")] 
    public float cost = 1000;
    public float exp = 1000;
    public int HP = 10;
    public int place = 10;
}
