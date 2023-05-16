using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds = null; // Array que contiene todos los sonidos del juego

    public static AudioManager instance;
    [SerializeField]
    AudioMixerGroup sfx;

    public enum ESounds { Swing, Menu, MatonShot, Bloqueo1, Bloqueo2, Bloqueo3, FlasherRay, Level1, Hit, 
                        Level2, Boss, Level1Low, Level2Low, RalentTime, RalentExp, RompePared, TurretShot,
                        Bastonazo, TurretWalk, RalenTurretDeath, DronExp, PrestShot, CrystalBreak, Credits}; // Enum usado para acceder al array sounds

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else        
            Destroy(this.gameObject);
        

        foreach (Sound s in sounds) // Asigna a cada sonido un AudioSource con las características correspondientes
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip; 
            s.source.outputAudioMixerGroup = s.mixer; 
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
    }

    public void Play (ESounds sound) // Hace sonar el sonido que corresponda
    {
        int i = (int)sound;
        Sound s = sounds[i];
        if(!s.source.isPlaying)
            s.source.Play();
    }

    public void Stop (ESounds sound) // Para el sonido que corresponda
    {
        int i = (int)sound;
        Sound s = sounds[i];
        if(s.source.isPlaying)
            s.source.Stop();
    }

    public void StopAll() // Para todos los sonidos
    {
        Sound s;
        for (int i = 0; i < sounds.Length; i++)
        {
            s = sounds[i];
            if (s.source.isPlaying)
                s.source.Stop();
        }
    }

    public void StopAllSFX() // Para todos los efectos de sonido
    {
        Sound s;
        for(int i = 0; i < sounds.Length; i++)
        {
            s = sounds[i];
            if (s.source.outputAudioMixerGroup == sfx) 
                s.source.Stop();
        }
    }

    public bool IsPlaying(ESounds sound) // Comprueba si un clip está sonando o no
    {
        int i = (int)sound;
        Sound s = sounds[i];
        if (s.source.isPlaying)
            return true;
        else
            return false;
    }
}


