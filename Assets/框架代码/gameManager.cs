using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    private static gameManager instance;
    
    // 所有干员operData
    public List<operData> AllOperData_p = new List<operData>();
    public static List<operData> AllOperData;

    // 编队相关
    public static List<operData>[] formation = new List<operData>[4];   // 预设的4个编队
    public static int formationNum;     // 当前选择的编队编号
    
    // 持有的摩拉袋数量
    public static int Mora=10;
    // 当前的罐装知识数据
    public static canningKnowledgeData knowledgeData = new canningKnowledgeData();
    public static canningKnowledgeData_Strengthen knowledgeDataStrengthen = new canningKnowledgeData_Strengthen();
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);

        AllOperData = AllOperData_p;
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
}
