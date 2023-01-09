using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class rolWhenMouseOn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 localRol = new Vector3();
    public float turnSpeed = 10;

    private Quaternion finalRol = new Quaternion();
    private Quaternion zeroRol = new Quaternion();
    private Quaternion tarRol = new Quaternion();
    
    void Start()
    {
        finalRol = Quaternion.Euler(localRol);
        zeroRol = Quaternion.Euler(0, 0, 0);
        tarRol = zeroRol;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation,tarRol,Time.deltaTime * turnSpeed);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        tarRol = finalRol;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tarRol = zeroRol;
    }
    
}
