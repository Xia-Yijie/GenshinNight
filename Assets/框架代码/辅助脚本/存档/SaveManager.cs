using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    public static void Save(int id)
    {
        CreateDicByID(id);
        SaveData save = new SaveData();
        save.DownLoad();
        
        string JsonString = JsonUtility.ToJson(save);

        StreamWriter sw = new StreamWriter(GetDataPathByID(id));
        sw.Write(JsonString);
        sw.Close();
        
        // 小数据写入
        SaveDataSmall saveSmall = new SaveDataSmall();
        saveSmall.DownLoad();
        
        string JsonStringSmall = JsonUtility.ToJson(saveSmall);

        StreamWriter sws = new StreamWriter(GetDataPathByID(id, true));
        sws.Write(JsonStringSmall);
        sws.Close();
    }
    
    
    public static void Load(int id)
    {
        CreateDicByID(id);
        string path = GetDataPathByID(id);
        if(File.Exists(path))
            //判断文件是否创建
        {
            StreamReader sr=new StreamReader(path);
            //从流中读取字符串
            string JsonString=sr.ReadToEnd();
            sr.Close();
            SaveData save=JsonUtility.FromJson<SaveData>(JsonString);
            
            save.UpLoad();
        }
        else{
            Debug.LogError("File Not Found.");
        }
    }

    public static SaveDataSmall Load_Small(int id)
    {
        CreateDicByID(id);
        string path = GetDataPathByID(id, true);
        if (!File.Exists(path)) return null;
        
        StreamReader sr=new StreamReader(path);
        //从流中读取字符串
        string JsonString=sr.ReadToEnd();
        sr.Close();
        SaveDataSmall save=JsonUtility.FromJson<SaveDataSmall>(JsonString);
        return save;
    }

    private static void CreateDicByID(int id)
    {
        string path = gameManager.SaveDataPath + "/save_" + id;
        if (Directory.Exists(path)) return;
        Directory.CreateDirectory(path);
    }

    public static string GetDataPathByID(int id, bool isSmall = false)
    {
        if (isSmall) return gameManager.SaveDataPath + "/save_" + id + "/small";
        return gameManager.SaveDataPath + "/save_" + id + "/data";
    }
    
}
