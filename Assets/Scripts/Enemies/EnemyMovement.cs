using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Asociado al enemigo. Si no está activo, el enemigo permanece quieto. En caso contrario, el enemigo se mueve con 
// velocidad "speed" rodeando al jugador. Cada medida de tiempo "changeDir", decidirá mediante el booleano "clockwise"
// si continuará la ruta que está siguiendo o cambiará de sentido

[RequireComponent(typeof(Rigidbody2D))]

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] float speed = 0.5f;
    [SerializeField] float firstMove = 0, changeDir = 2.5f;
    bool clockwise;
    Vector2 direction;
    Rigidbody2D rb;
    EnemyVision vision;
    Animator anim;
    Transform child;
    EnemyVision enemy;
    Drone drone;
    float time;

    void Start()
    {
        enemy = GetComponent<EnemyVision>();
        drone = GetComponent<Drone>();
        if (enemy != null && drone == null)
            child = transform.GetChild(0);
        anim = child.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        vision = GetComponent<EnemyVision>();
        clockwise = false;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (player != null)
        {
            // Cuando "Time.time" alcanza el nuevo valor de "firstMove" da un valor aleatorio a "selecDir" entre 0 y 1,
            // que determinarán si el enemigo girará en el sentido de las agujas del reloj o en sentido contrario mediante 
            // el booleano "clockwise". Aumenta el valor de "firstMove" mediante la variable "changeDir" 
            if (time > firstMove && vision.Spotted(player))
            {
                int selecDir = Random.Range(0, 2);

                if (selecDir == 0)
                    clockwise = true;
                else
                    clockwise = false;

                firstMove = changeDir; // TIME.TIME -----------------------------------> USAR TIME.DELTATIME
                time = 0;
            }

            if (vision.Spotted(player))
            {

                // "direction" será dado por "clockwise"
                if (clockwise)
                    direction = new Vector2(transform.position.y - player.position.y, player.position.x - transform.position.x);
                else
                    direction = new Vector2(player.position.y - transform.position.y, transform.position.x - player.position.x);
            }
            else
                direction = Vector2.zero;
        }
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
    }

    // Movimiento del enemigo con dirección "direction" y velocidad "speed"
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }
}
