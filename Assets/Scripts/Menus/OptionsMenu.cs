using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    AudioMixer master = null; // Utilizamos el mixer para controlar el volumen
    
    [SerializeField]
    GameObject pauseMenuUI = null, optionsMenuUI = null, pauseFirstButton = null, mainFirstButton = null; // Referencian los demás menus y que botón debería estar seleccionado al volver a ellos

    [SerializeField]
    GameObject mainVolSlider, SFXVolSlider, musicVolSlider, fulscreenToggle, deathToggle, controlToggle, aButton, spacebar;

    private void Start()
    {
        mainVolSlider.GetComponent<Slider>().value = GameManager.instance.mainVolSlider;
        SFXVolSlider.GetComponent<Slider>().value = GameManager.instance.SFXVolSlider;
        musicVolSlider.GetComponent<Slider>().value = GameManager.instance.musicVolSlider;
        fulscreenToggle.GetComponent<Toggle>().isOn = Screen.fullScreen;
        controlToggle.GetComponent<Toggle>().isOn = GameManager.instance.mando;
        //deathToggle.GetComponent<Toggle>().isOn = !GameManager.instance.toggleDeath;

        // Activa la imagen del botón A o el espacio en los diálogos
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            aButton.SetActive(GameManager.instance.mando);
            spacebar.SetActive(!GameManager.instance.mando);
        }
    }

    public void SetMasterVolume(float volume) // Slider del volumen general
    {
        master.SetFloat("masterVolume", Mathf.Log10 (volume) * 20);
        GameManager.instance.MainSliderState(volume);
    }

    public void SetMusicVolume(float volume) // Slider del volumen de la música
    {
        master.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
        GameManager.instance.MusicSliderState(volume);
    }

    public void SetSFXVolume (float volume) // Slider del volumen de los efectos
    {
        master.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        GameManager.instance.SFXSliderState(volume);
    }

    public void SetFullScreen(bool isFullscreen) // Controla si el juego debe estar a pantalla completa o no
    {
        Screen.fullScreen = isFullscreen;
        GameManager.instance.FullscreenToggleState(isFullscreen);
    }

    public void Back() // Es llamado al salir del menú de opciones
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
        else if (mainFirstButton != null)
            EventSystem.current.SetSelectedGameObject(mainFirstButton);
       optionsMenuUI.SetActive(false);
    }
    public void ControlToggle(bool mando) // Botón para cambiar entre control con mando o con teclado
    {
        if(aButton!=null)
            aButton.SetActive(mando); // Activa la aparición del botón A en los cuadros de diálogo
        if (spacebar != null)
            spacebar.SetActive(!mando); // Activa la aparición de la barra espaciadora en los cuadros de diálogo
        GameManager.instance.ControlToggle(mando); 
    }
    /*public void DeathToggle(bool death) // God mode only
    {
        GameManager.instance.DeathToggle(death);
    }*/
}
