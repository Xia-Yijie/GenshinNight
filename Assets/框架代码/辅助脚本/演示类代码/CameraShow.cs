using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShow : MonoBehaviour
{
    public Camera mainCamera;
    public Camera UICamera;
    public float view = 40;
    public Vector3 pos = new Vector3(0, 10, 0);

    private float preView;
    private Vector3 prePos;
    private bool isBig = false;
    private Vector3 tarPos;
    private float tarView;
    private float speed = 8;

    private void Start()
    {
        CameraController cc = mainCamera.GetComponent<CameraController>();
        cc.enabled = false;
        tarView = preView = mainCamera.fieldOfView;
        tarPos = prePos = mainCamera.transform.position;
    }

    void Update()
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, tarView, speed * Time.unscaledDeltaTime);
        UICamera.fieldOfView = Mathf.Lerp(UICamera.fieldOfView, tarView, speed * Time.unscaledDeltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, tarPos, speed * Time.unscaledDeltaTime);
        
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (isBig)
            {
                tarView = view;
                tarPos = pos;
                isBig = false;
            }
            else
            {
                tarView = preView;
                tarPos = prePos;
                isBig = true;
            }
        }
    }


}
