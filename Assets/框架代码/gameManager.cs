using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    private static gameManager instance;
    
    // 所有干员operData
    public List<operData> AllOperData_p = new List<operData>();
    public static List<operData> AllOperData;
    public static Dictionary<string, bool> AllOperValid = new Dictionary<string, bool>();
    public static int UnlockCharacterNum = 0;


    // 编队相关
    public static List<operData>[] formation = new List<operData>[4];   // 预设的4个编队
    public static int formationNum;     // 当前选择的编队编号
    
    // 持有的摩拉袋和原石数量
    public static int Mora { get; private set; } = 2000;
    public static int Primogem { get; private set; } = 800000;
    
    // 当前的罐装知识数据
    public static canningKnowledgeData knowledgeData = new canningKnowledgeData();
    public static canningKnowledgeData_Strengthen knowledgeDataStrengthen = new canningKnowledgeData_Strengthen();
    
    // 通用存档地址
    public static string SaveDataPath; 
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        SaveDataPath = Application.persistentDataPath + "/SaveData";

        AllOperData = AllOperData_p;
        foreach (var od in AllOperData)
        {
            AllOperValid.Add(od.EnName, false);
        }
        
        for (int i = 0; i < 4; i++) formation[i] = new List<operData>();
    }
    
    void Start()
    {
        // SaveManager.Save();
        // SaveManager.Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static operData GetOperDataByEnName(string EnName)
    {
        return AllOperData.FirstOrDefault(od => od.EnName == EnName);
    }


    public static void SetPrimogem(int v)
    {
        Primogem = v;
    }
    public static void GetPrimogem(int v)
    {
        Primogem += v;
    }

    public static void SetMora(int v)
    {
        Mora = v;
    }
    public static void GetMora(int v)
    {
        Mora += v;
    }
    
}
