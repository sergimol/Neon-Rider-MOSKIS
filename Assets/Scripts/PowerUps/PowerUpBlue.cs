using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBlue : MonoBehaviour
{
    private PlayerController movi;
    private float fixedDeltaTime;
    Animator [] anim;
    private void Awake()
    {
        //Guardamos el DeltaTime inicial
        this.fixedDeltaTime = Time.fixedDeltaTime;
        anim = GetComponentsInChildren<Animator>();
    }
    void OnEnable()
    {
        //Al activarse triplicamos la velocidad del player y reducimos el timeScale a un tercio
        movi = gameObject.GetComponent<PlayerController>();
        movi.MulSpeed(3);
        anim[0].speed=3;
        anim[1].speed = 3;
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    private void OnDisable()
    {
        //Al desactivarse devolvemos todos los valores a su estado inicial.
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        anim[0].speed = 1;
        anim[1].speed = 1;
        movi = gameObject.GetComponent<PlayerController>();
        movi.DivSpeedReset(3);
    }
}
