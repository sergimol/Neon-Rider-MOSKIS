using System.Diagnostics;
using UnityEngine;

// Asociado a la bala. Toma la posición del jugador y se mueve hacia ella con una velocidad constante

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 20f;

    Transform player;
    
    Enemy_Death death;
    Rigidbody2D rb;
    void Start() // Toma referencia del jugador para orientarse a él 
    {
        if(transform.parent != null)    
         player = GetComponentInParent<EnemyAttack>().player;
        if (player != null) 
        {
            AudioManager.instance.Play(AudioManager.ESounds.MatonShot); // Sonido de disparo
            transform.parent = null;
            rb = GetComponent<Rigidbody2D>();
            rb.velocity = (player.position - transform.position).normalized * speed;       
        }
    }
    //Permite matar al jugador o al enemigo cuando colisionan con la bala
    //La bala es destruida al colisionar con cualquier cosa.
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        death = collision.gameObject.GetComponent<Enemy_Death>();
        
        if (death != null)
        {
            death.OnAttack();
        }

        Death dead = collision.gameObject.GetComponent<Death>();
        if (dead != null)
            dead.Dead();

        Destroy(this.gameObject);
    }


}
