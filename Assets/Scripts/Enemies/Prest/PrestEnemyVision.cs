using UnityEngine;

// Asociado al enemigo. Toma la capa en la que éste y el jugador se encuentran, pasa su valor a binario y los utiliza, mediante 
// Physics2D.Raycast y la dirección del jugador previamente tomada para determinar si el jugador está en el rango de visión del 
// enemigo. Si es el caso, se activa el ataque y el movimiento

//IDENTICO A VISIÓN?

public class PrestEnemyVision : MonoBehaviour
{
    public Transform player;
    EnemyAttack attack;
    [SerializeField] int range = 5;
    [SerializeField] int playerLayer = 8, wallsLayer = 9;
    Vector2 direccion;
    RaycastHit2D hit;
    float distance;

    void Start()
    {
        attack = GetComponent<EnemyAttack>();
    }

    void Update()
    {
        if (player != null)
        {
            // Pasa el valor de las capas a binario y las junta en la variable "maskLayer"
            // con un operador OR de manera que se comprueben ambas capas
            int playerMask = 1 << playerLayer; 
            int wallsMask = 1 << wallsLayer;
            int maskLayer = playerMask | wallsMask;

            // Toma la dirección del jugador repecto al enemigo y, si algún objeto perteneciente a las capas 8 o 9 está 
            // dentro del rango de distancia "range", guarda el más cercano en la variable hit
            direccion = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
            distance = Mathf.Sqrt((direccion.x * direccion.x) + (direccion.y * direccion.y));
            if (distance <= range)  
                hit = Physics2D.Raycast(transform.position, direccion, range, maskLayer);
            else
                hit = Physics2D.Raycast(transform.position, direccion, range, wallsMask);
            // Cuando el hit es igual al jugador, activa el comportamiento del enemigo. En caso contrario lo desactiva
            if (hit.transform==player)
            {
                attack.enabled = true;
            }
            else
            {
                attack.enabled = false;
            }
        }
        else
        {
            attack.enabled = false;
        }
    }
}
