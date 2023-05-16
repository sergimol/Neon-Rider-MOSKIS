using System.Threading;
using UnityEngine;

// Componente utilizado para activar power-up's del jugador


public class ActivatePowerUpGreenBlue : MonoBehaviour
{
    [SerializeField] GameObject player = null;
    PowerUpManager pum;
    float delay = 0.1f;
    float time = 0;

    // Variable de los PowerUps Verde y Azul
    Ralentizador ralentizador;

    void Start()
    {
        
        // Se crea un delay mínimo para que no se active al reiniciar 
        // escena si se destuyen por checkpoint

        //Inicializa el PowerUpManager
        if (player != null)
            pum = player.GetComponent<PowerUpManager>();
        //Proteccion contra no inicializacion de PowerUpManager
        if (pum == null)
        {
            Debug.Log("Jugador sin gestor de power-ups." + " Se ignora el power-up conseguido.");
        }

        ralentizador = GetComponent<Ralentizador>();
        time = 0;
    }

    private void Update()
    {
        time += Time.deltaTime;
    }

    void OnDestroy()
    {
        if (pum != null && time > delay)
        {

            //Si se destruye con la espada activa el azul
            if (ralentizador.GetTime() > 0)
            {
                pum.ActivatePowerUp("PowerUpBlue");
            }
            //Si explota activa el v erde
            else
            {
                pum.ActivatePowerUp("PowerUpGreen");
            }

        }

    }
}