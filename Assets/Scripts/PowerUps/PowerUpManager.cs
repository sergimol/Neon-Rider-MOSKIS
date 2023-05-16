using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente pensado para ser añadido al jugador para gestionar
/// sus power-up's.
/// </summary>
/// Cada power-up es implementado con un componente distinto.
/// Un power-up estará habilitado si su componente está activado.
/// Por lo tanto todos los componentes que implementan power-up's
/// estarán deshabilitados desde el principio.
/// Este componente (en el jugador) añade un método ActivatePowerUp(string)
/// que recibe el nombre del power-up a activar (debe coincidir con
/// el nombre del componente) y lo activa. Si había un power-up previo
/// activo, lo desactiva.
public class PowerUpManager : MonoBehaviour
{

    MonoBehaviour currentPowerUp;
    [SerializeField] float duration = 5f;
    PowerUpRed red;
    PowerUpBlue blue;
    PowerUpGreen green;
    PowerUpPurple purple;
    PowerUpYellow yellow;

    public Image image;
    [SerializeField] Sprite redIndicator = null;
    [SerializeField] Sprite blueIndicator = null;
    [SerializeField] Sprite greenIndicator = null;
    [SerializeField] Sprite purpleIndicator = null;
    [SerializeField] Sprite yellowIndicator = null;

    ActivatePowerUpPurple activPowPurple;

    SpriteRenderer [] neonSword;

    float width, originalWidth, time;
    bool activo = false;
    float clock = 0;

    void Awake()
    {
        width = image.rectTransform.sizeDelta.x;
        originalWidth = width;
        // Cogemos los scripts de los Powerups
        red = GetComponent<PowerUpRed>();
        blue = GetComponent<PowerUpBlue>();
        green = GetComponent<PowerUpGreen>();
        purple = GetComponent<PowerUpPurple>();
        yellow = GetComponent<PowerUpYellow>();
        activPowPurple = GetComponent<ActivatePowerUpPurple>();
        // Cogemos el sprite de la espada para poder cambiarlo de color luego
        neonSword = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        //Actualizamos la variable de control de la duración
        clock += Time.deltaTime;    
        //Si no hay ningún PU activo activa el indicador correspondiente
        if (!activo)                    
        {
            if (red.enabled)
            {
                ActivateIndicator(Color.red, redIndicator);
            }
            else if (blue.enabled)
            {
                ActivateIndicator(Color.blue, blueIndicator);
            }
            else if (green.enabled)
            {
                ActivateIndicator(Color.green, greenIndicator);
            }
            else if (yellow.enabled)
            {
                ActivateIndicator(Color.yellow, yellowIndicator);
            }
            else if (purple.enabled)
            {
                ActivateIndicator(new Color(177, 0, 255, 255), purpleIndicator);
            }
        }
        //si el tamaño de la barra es 0 desactiva la imagen de la barra 
        if (width <= 0)
        {
            neonSword[1].color = Color.black;
            //Indicator
            neonSword[2].color = Color.white;
            activo = false;
            
            if (red.enabled)
            {
                DeactivatePowerUp("PowerUpRed");
            }
            else if (blue.enabled)
            {
                DeactivatePowerUp("PowerUpBlue");
            }
            else if (green.enabled)
            {
                DeactivatePowerUp("PowerUpGreen");
            }
            else if (purple.enabled)
            {
                DeactivatePowerUp("PowerUpPurple");
                activPowPurple.enabled = true;
            }
            else if (yellow.enabled)
            {
                DeactivatePowerUp("PowerUpYellow");
            }
        }
        // si el tamaño no es 0 disminuñe el tamaño de la barra a la velocidad correspondiente de cada power up
        else
        {
            image.rectTransform.sizeDelta = new Vector2(width, image.rectTransform.sizeDelta.y);
            if (currentPowerUp == null)
            {
                width = 0;
            }
            else
            {
                if (blue.enabled)
                {
                    width = (originalWidth * ((time - clock) / (duration / 3)));
                }
                else
                    width = originalWidth * ((time - clock) / duration);
            }
        }            
    }

    public void ActivatePowerUp(string powerUpName)
    {

        // Localiza el componente powerUpName asociado 
        MonoBehaviour powerUp = GetComponent(powerUpName) as MonoBehaviour;

        if (powerUp == null)
        {
            Debug.Log("Componente power-up " + powerUpName + " no encontrado. Se ignora.");
        }
        else
        {
            // Si no está activo
            if (powerUp != currentPowerUp)
            {
                // Desactiva el power-up activo y su indicador
                if (currentPowerUp != null)
                {
                    Debug.Log("Desactivo");
                    currentPowerUp.enabled = false;
                    image.enabled = false;
                    activo = false;
                }

                // Activa el power-up indicado
                powerUp.enabled = true;
                Debug.Log("Componente power-up " + powerUpName + " activado.");

                currentPowerUp = powerUp;

                // Activa el indicador del Powerup
                if (image != null)
                    image.enabled = true;
            }
            // Añade a timeDuratión la duración del Powerup
            if (powerUpName == "PowerUpBlue")
            {
                time = duration / 3;
                clock = 0;
            }
            else
            {
                time = duration;
                clock = 0;
            }
                
        }
    }

    void DeactivatePowerUp(string powerUpName)
    {

        // Localiza el componente powerUpName asociado 
        MonoBehaviour powerUp = GetComponent(powerUpName) as MonoBehaviour;

        if (powerUp == null)
        {
            Debug.Log("Componente power-up " + powerUpName + " no encontrado. Se ignora.");
        }
        else
        {
            if (currentPowerUp != null)     // Desactiva el power-up activo y resetea las variables de control
            {
                currentPowerUp.enabled = false;
            }
            activo = false;
            currentPowerUp = null;
            image.enabled = false;
            Debug.Log("Componente power-up " + powerUpName + " Desactivado.");
        }
    }

    void ActivateIndicator(Color color, Sprite indicator)
    {
        neonSword[1].color = color;
        //Indicator
        neonSword[2].color = color;
        width = originalWidth;
        image.sprite = indicator;
        image.enabled = true;
        activo = true;
    }
}