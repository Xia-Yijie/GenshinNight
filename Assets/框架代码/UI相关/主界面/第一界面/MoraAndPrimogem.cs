using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoraAndPrimogem : MonoBehaviour
{
    public Text moraText;
    public Text primogemText;

    private void Update()
    {
        moraText.text = gameManager.Mora.ToString();
        primogemText.text = gameManager.Primogem.ToString();
    }
}
