using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward_Show : MonoBehaviour
{
    public Vector3 direction = new Vector3(1, 0, 0);
    public float speed = 2;

    private void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }
}
