using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager
{
    // 对象池
    private const int maxCount = 64;
    private static Dictionary<enemyInfo, List<EnemyCore>> pool = new Dictionary<enemyInfo, List<EnemyCore>>();
    private static Dictionary<enemyInfo, GameObject> objPrt = new Dictionary<enemyInfo, GameObject>();
    
    public static void RecycleEnemy(EnemyCore EC)
    {
        enemyInfo ei_ = EC.ei_;
        
        // 如果没有父物体则生成
        if (!objPrt.ContainsKey(ei_))
        {
            objPrt.Add(ei_, new GameObject(ei_.Name + "对象池"));
        }
        
        EC.transform.SetParent(objPrt[ei_].transform);
        EC.gameObject.SetActive(false);

        if (pool.ContainsKey(ei_))
        {
            if (pool[ei_].Count < maxCount)
            {
                pool[ei_].Add(EC);
            }
            else Debug.Log("超过敌人对象池最大上线");
        }
        else
        {
            pool.Add(ei_, new List<EnemyCore>() { EC });
        }
    }
    
    public static EnemyCore GetEnemy(enemyInfo ei_)
    {
        // 如果没有父物体则生成
        if (!objPrt.ContainsKey(ei_))
        {
            objPrt.Add(ei_, new GameObject(ei_.Name + "对象池"));
        }

        // 池子中有
        EnemyCore result = null;
        if (pool.ContainsKey(ei_))
        {
            if (pool[ei_].Count > 0)
            {
                result = pool[ei_][0];
                pool[ei_].Remove(result);
                result.transform.SetParent(objPrt[ei_].transform);
                result.gameObject.SetActive(true);
                return result;
            }
        }
        // 池子中缺少
        result = Object.Instantiate(ei_.enemyPrefab, objPrt[ei_].transform).GetComponent<EnemyCore>();
        RecycleEnemy(result);
        GetEnemy(ei_);
        return result;
    }

    public static void Clear()
    {
        pool.Clear();
        objPrt.Clear();
    }
}
