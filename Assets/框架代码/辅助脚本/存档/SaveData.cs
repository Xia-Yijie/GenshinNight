using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SaveData
{
    public string PlayerID;
    public int Mora;
    public int Primogem;
    public canningKnowledgeData KnowledgeData = new canningKnowledgeData();
    public canningKnowledgeData_Strengthen KnowledgeDataStrengthen = new canningKnowledgeData_Strengthen();
    public List<string> AllValidOperName = new List<string>();
    public int UnlockCharacterNum;
    public List<string> formation0 = new List<string>();
    public List<string> formation1 = new List<string>();
    public List<string> formation2 = new List<string>();
    public List<string> formation3 = new List<string>();
    

    public void DownLoad()
    {// 让本数据与内存数据相同
        Mora = gameManager.Mora;
        Primogem = gameManager.Primogem;
        KnowledgeData = gameManager.knowledgeData;
        KnowledgeDataStrengthen = gameManager.knowledgeDataStrengthen;
        AllValidOperName.Clear();
        foreach (var pair in gameManager.AllOperValid.Where(pair => pair.Value))
        {
            AllValidOperName.Add(pair.Key);
        }
        UnlockCharacterNum = gameManager.UnlockCharacterNum;

        for (int i = 0; i < 4; i++)
        {
            var formation = i switch
            {
                0 => formation0,
                1 => formation1,
                2 => formation2,
                3 => formation3,
            };

            formation.Clear();
            foreach (var od in gameManager.formation[i])
            {
                formation.Add(od.EnName);
            }
        }
    }
    
    public void UpLoad()
    {// 让内存数据与本数据相同
        gameManager.Mora = Mora;
        gameManager.Primogem = Primogem;
        gameManager.knowledgeData = KnowledgeData;
        gameManager.knowledgeDataStrengthen = KnowledgeDataStrengthen;
        gameManager.UnlockCharacterNum = UnlockCharacterNum;
        gameManager.AllOperValid.Clear();
        foreach (var na in AllValidOperName)
        {
            gameManager.AllOperValid.Add(na, true);
        }
        foreach (var od in gameManager.AllOperData)
        {
            if (gameManager.AllOperValid.ContainsKey(od.EnName)) continue;
            gameManager.AllOperValid.Add(od.EnName, false);
        }

        for (int i = 0; i < 4; i++) 
        {
            var formation = i switch
            {
                0 => formation0,
                1 => formation1,
                2 => formation2,
                3 => formation3,
            };
            
            gameManager.formation[i].Clear();
            foreach (var EnName in formation)
            {
                gameManager.formation[i].Add(gameManager.GetOperDataByEnName(EnName));
            }
        }
    }
    
}

public class SaveDataSmall
{
    public int Mora;
    public int Primogem;
    
    public void DownLoad()
    {
        Mora = gameManager.Mora;
        Primogem = gameManager.Primogem;
    }
}