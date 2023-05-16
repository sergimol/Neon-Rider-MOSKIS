using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckpoint : MonoBehaviour
{
    public int deadVal = 0;
    [SerializeField]
    private Vector2 pos;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            // Transforma ésta en la posición de respawn del jugador
            // y se encarga de evitar que revivan los enemigos anteriores
            GameManager.instance.checkpoint = pos;
            if (deadVal > GameManager.instance.deadVal)
                GameManager.instance.deadVal = deadVal;
        }
    }
}
