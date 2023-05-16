using UnityEngine.UI;
using UnityEngine;

public class PowerUpYellow : MonoBehaviour
{
    //imagen, necesario publico para modificarlo desde el jugador (quien tiene el script)
    public Image image; 
    //la activa cuando el power up esta enbled y lo desactiva si no.
    private void OnEnable()
    {
        if(image != null)
            image.enabled = true;
    }
    private void OnDisable()
    {
        if(image!= null)
            image.enabled = false;
    }
}
