using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    Transform arriba, abajo, medio;

    // Los trozos de la pared que quedan cuando se rompe se asignan a las variables arriba y abajo
    private void Start()
    {
        arriba = transform.GetChild(0); 
        abajo = transform.GetChild(1);
        medio = transform.GetChild(2);
    }

    // Cuando la pared se rompa libera a sus hijos y se destruye la parte entera
    public void Break()
    {
        arriba.GetComponent<Renderer>().enabled=true;
        abajo.GetComponent<Renderer>().enabled = true;
        medio.GetComponent<Renderer>().enabled = true;
        arriba.SetParent(null);
        abajo.SetParent(null);
        medio.SetParent(null);
        Destroy(this.gameObject);
    }
}
