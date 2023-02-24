using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    public static void Save()
    {
        SaveData save = new SaveData();
        save.Mora = 20;
        save.canningKnowledgeData_.atkInc.maxNum = 10;
        save.UnlockOperList.Add(true);
        
        string JsonString = JsonUtility.ToJson(save);
        
        // Debug.Log(Application.dataPath + "/SaveData.yjc");
        StreamWriter sw = new StreamWriter(Application.dataPath + "/SaveData.yjc");
        sw.Write(JsonString);
        sw.Close();
    }
    
    
    public static void Load()
    {
        if(File.Exists(Application.dataPath+"/SaveData.yjc"))
            //判断文件是否创建
        {
            StreamReader sr=new StreamReader(Application.dataPath+"/SaveData.yjc");
            //从流中读取字符串
            string JsonString=sr.ReadToEnd();
            sr.Close();
            SaveData save=JsonUtility.FromJson<SaveData>(JsonString);
            
            // 复制摩拉袋数据
            gameManager.Mora = save.Mora;
            // 复制两个罐装知识数据（指针）
            gameManager.knowledgeData = save.canningKnowledgeData_;
            gameManager.knowledgeDataStrengthen = save.canningKnowledgeDataStrengthen;
            
        }
        else{
            Debug.LogError("File Not Found.");
        }
    }
}
