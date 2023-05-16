using UnityEngine;
//Activa el potenciador morado
public class ActivatePowerUpPurple : MonoBehaviour
{
    PowerUpManager pum;
    Bloqueo parry;
    //Cogemos la referencia al PUmanager
    void Start()
    {
        pum = GetComponent<PowerUpManager>();
        if (pum == null)
        {
            Debug.Log("Jugador sin gestor de power-ups." + " Se ignora el power-up conseguido.");
        }
    }

    //Cuando una bala morada (prestbullet) entra al trigger de la espada y el bloqueo esta activo , se activa el morado
    //Se desactiva automaticamente para que se pueda volver a activar el morado al bloquear otra bala morada
    void OnTriggerEnter2D(Collider2D collision)
    {
        parry = GetComponent<Bloqueo>();
        if (collision.gameObject.GetComponent<PrestBullet>()!=null && parry.enabled)
        {
            pum.ActivatePowerUp("PowerUpPurple");
            this.enabled = false;
        }
    }
}
