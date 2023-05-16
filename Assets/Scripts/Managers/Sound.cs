using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound // Determina las características de cada clip de sonido del juego
{
    public string name; // Nombre

    public AudioClip clip; // Clip de audio
    public AudioMixerGroup mixer; // Mezclador de sonido correspondiente al clip

    [Range(0f, 1f)]
    public float volume; // Volumen

    public bool loop; // Si debe estar o no en bucle

    [HideInInspector]
    public AudioSource source; // AudioSource para reproducir el clip
}