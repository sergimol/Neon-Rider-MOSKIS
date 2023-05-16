using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGreen : MonoBehaviour
{
    private PlayerController movi;
    void OnEnable()
    {
        //Al activarse reduce la velocidad del jugador
        movi=gameObject.GetComponent<PlayerController>();
        movi.DivSpeedReset(2);
    }
    private void OnDisable()
    {
        //Al desactivarse se devuelve la velocidad a su valor inicial
        movi = gameObject.GetComponent<PlayerController>();
        movi.MulSpeed(2);
    }
}
