using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string PlayerID;
    public int Mora;
    public canningKnowledgeData canningKnowledgeData_ = new canningKnowledgeData();
    public canningKnowledgeData_Strengthen canningKnowledgeDataStrengthen = new canningKnowledgeData_Strengthen();
    public List<bool> UnlockOperList = new List<bool>();


}
