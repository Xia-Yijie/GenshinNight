using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class video : MonoBehaviour
{
    private Animator anim;
    private float speed = 1.2f;
    private int sta = 0;
    private float[] t = {4f,1f,1f,2f}; 

    private Vector3 forward = new Vector3(1, 0, 0);
    private Vector3 backword = new Vector3(-1, 0, 0);
    private Vector3 backScale = new Vector3(-0.75f, 0.75f, 0.75f);
    
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        if (sta >= t.Length) return;
        
        if (t[sta] <= 0)
            sta++;

        if (sta >= t.Length) return;

        t[sta]-=Time.deltaTime;
        
        // switch (sta)
        // {
        //     case 0:
        //         anim.SetBool("move", true);
        //         transform.Translate(forward * (speed * Time.deltaTime));
        //         break;
        //     case 1:
        //         anim.SetBool("move", false);
        //         break;
        //     case 2:
        //         transform.localScale =
        //             Vector3.Lerp(anim.transform.localScale, backScale, 10 * Time.deltaTime);
        //         break;
        //     case 3:
        //         anim.SetBool("move", true);
        //         transform.Translate(backword * (speed * Time.deltaTime));
        //         break;
        // }

        switch (sta)
        {
            case 0:
                anim.SetBool("move", true);
                transform.Translate(backword * (speed * Time.deltaTime));
                break;
            case 1:
                anim.SetBool("move", false);
                break;
        }
        
    }
}
