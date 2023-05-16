
using UnityEngine;
using UnityEngine.SceneManagement;
//Reinicia la escena al morir 
public class ResetScene : MonoBehaviour
{
    private void Update()
    {
        //Cuando el sprite de la animacion del objeto (muertoPlayer) llega al indicado se recarga la escena
        if (GetComponent<SpriteRenderer>().sprite.name == "EndDeath")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
