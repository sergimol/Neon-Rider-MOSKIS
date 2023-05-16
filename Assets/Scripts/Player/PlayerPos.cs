using UnityEngine;

public class PlayerPos : MonoBehaviour
{
    void Start()
    {
        transform.position = GameManager.instance.checkpoint; // Mueve al player a la posición del último checkpoint
    }
}