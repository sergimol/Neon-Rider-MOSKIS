using UnityEngine;

public class ActivatePowerUpYellow : MonoBehaviour
{
    PowerUpManager pum;
    void Start()
    {
        //Inicializa el PowerUpManager
        pum = GetComponentInParent<PowerUpManager>();
        //Proteccion contra no inicializacion de PowerUpManager
        if (pum == null)
        {
            Debug.Log("Jugador sin gestor de power-ups." + " Se ignora el power-up conseguido.");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {       
        //El jugador detecta que es un rayo amarillo y activa el powerup amarillo
        if(pum != null && other.GetComponent<FlasherRay>() != null)
        {
            pum.ActivatePowerUp("PowerUpYellow");
        }
    }
}
