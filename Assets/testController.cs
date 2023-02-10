using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testController : MonoBehaviour
{
    public operData dadaliya;
    public operData kroos;
    public operData steward;
    public operData beagle;
    public operData melantha;
    public operData orchid;
    public operData catapult;
    public operData itto;
    public operData raidenShogun;
    public operData wanderer;
    public operData ganyu;
    public operData Jean;
    public operData Yelan;
    public operData lumine;

    private void Awake()
    {
        // InitManager.Register(kroos, 1);
        // InitManager.Register(steward, 1);
        // InitManager.Register(beagle, 1);
        // InitManager.Register(melantha, 1);
        // InitManager.Register(orchid, 1);
        // InitManager.Register(catapult, 1);
        InitManager.Register(dadaliya, 1);
        InitManager.Register(itto, 1);
        InitManager.Register(raidenShogun, 1);
        InitManager.Register(wanderer, 1);
        InitManager.Register(ganyu, 1);
        InitManager.Register(Jean, 1);
        InitManager.Register(Yelan, 1);
        InitManager.Register(lumine, 1);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
