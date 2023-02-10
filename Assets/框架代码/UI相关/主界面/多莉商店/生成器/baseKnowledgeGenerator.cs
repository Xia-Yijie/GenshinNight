using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class baseKnowledgeGenerator : MonoBehaviour
{
    public canningKnowledgeUI ckui;
    public canKnowSettingUI cksui;

    public abstract void GenerateUniversalKnowledge(bool isShop);

    public abstract void GenerateUniversalKnowledge_S(bool isShop);
}
