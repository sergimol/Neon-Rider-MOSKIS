using UnityEngine;
using UnityEngine.SceneManagement;
// *ENEMIGO*
// Una vez entra en el rango de visión del jugador, se activa la cuenta atrás
// Este script únicamente destruye el gameObject, los powerups se llaman desde sus respectivos scripts


public class Ralentizador : MonoBehaviour
{
    [SerializeField] GameObject ralentPRUEBA = null;
    [SerializeField] float time = 3.5f;
    private bool visto;
    //private Animator animator = GetComponentInChildren<Animator>();

    private void Awake()
    {
        visto = false;
        time = 3.5f;
    }
    void Update()
    {
        GetComponentInChildren<Animator>().SetBool("Visto", visto);
        Vector2 pos = new Vector2(Camera.main.WorldToViewportPoint(transform.position).x, 
                                  Camera.main.WorldToViewportPoint(transform.position).y);  // Coordenadas en cámara

        if (visto || pos.x <= 1 && pos.x >= 0 && pos.y <= 1 && pos.y >= 0) // Condición para que esté dentro de la cámara (visto para evitar parar contador)
        { 
            visto = true;
            time -= Time.deltaTime;
            if(time > 0 && !AudioManager.instance.IsPlaying(AudioManager.ESounds.RalentTime))
                AudioManager.instance.Play(AudioManager.ESounds.RalentTime);
            if (time <= 0)
            { // Explosión
                AudioManager.instance.Play(AudioManager.ESounds.RalentExp);
                Debug.Log("Exploto");
                if (SceneManager.GetSceneByName("Escena de Prueba 3") == SceneManager.GetActiveScene())
                    Instantiate(ralentPRUEBA);
                Destroy(this.gameObject);
            }
        }
    }

    public float GetTime() {
        return time;
    }
}
