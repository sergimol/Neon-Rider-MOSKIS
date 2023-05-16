using UnityEngine;
using UnityEngine.SceneManagement;

// Script de muerte del jugador

public class Death : MonoBehaviour
    // Meter al jugador para que muera
{
    [SerializeField] GameObject muerto = null;
    bool active = true;

    //habilita o desabilita la capacidad de morir.
    private void OnEnable() 
    {
        active = true;    
    }
    private void OnDisable()
    {
        active = false;
    }

    // Al ser invocado el método Dead() se considera al jugador muerto,
    // es eliminado y se invoca un "cadáver"
    public void Dead()
    {
        if (active)
        {
            Tracker.instance.AddEvent(new MuerteJugadorEvent(transform.position));
            AudioManager.instance.StopAllSFX();
            Instantiate(muerto, transform.position, Quaternion.identity);
            Destroy(this.gameObject);  
        }
    }
}
