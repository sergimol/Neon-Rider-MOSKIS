using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Asociado al enemigo. Si no está activo, el enemigo permanece quieto. En caso contrario, el enemigo se mueve con 
// velocidad "speed" rodeando al jugador. Cada medida de tiempo "changeDir", decidirá mediante el booleano "clockwise"
// si continuará la ruta que está siguiendo o cambiará de sentido

[RequireComponent(typeof(Rigidbody2D))]

public class PrestEnemyMovement : MonoBehaviour
{
    [SerializeField] Transform player = null;
    [SerializeField] float speed = 0.5f;
    EnemyVision vision;
    Vector2 direction;
    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        vision = GetComponent<EnemyVision>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetComponentInChildren<Animator>();
    }

    void Update() // Toma la dirección de movimiento respecto al player
    {
        if (player != null)
        {
            direction = new Vector2(transform.position.x - player.position.x, transform.position.y - player.position.y);
        }      
    }

    // Movimiento del enemigo con dirección "direction" y velocidad "speed"
    void FixedUpdate()
    {
        if (vision.Spotted(player))
            rb.velocity = direction * speed;
        else 
            rb.velocity = new Vector2(0, 0);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x + rb.velocity.y));
    }

    // Al salir el jugador del campo de visión, se queda quieto 
    void OnDisable()
    {
        rb.velocity = new Vector2(0,0);
    }
}
