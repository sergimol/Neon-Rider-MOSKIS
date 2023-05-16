using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsExit : MonoBehaviour
{
    float time = 150f; // Tiempo que duran los créditos

    void Update() // Cambia a la escena del menú cuando se acaban los créditos
    {
        if (time > 0)
            time -= Time.deltaTime;
        else
        {
            //GameManager.instance.OnSceneChange(0);
            SceneManager.LoadScene(0);
        }
    }
}
