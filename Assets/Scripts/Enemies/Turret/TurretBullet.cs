using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
// Asociado a la bala. Toma la posición del jugador y se mueve hacia ella con una velocidad constante

public class TurretBullet : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    Rigidbody2D rb;
  
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<TurretBullet>() == null){
            Destroy(this.gameObject);
        }
       
        Death death = collision.gameObject.GetComponent<Death>();
        if (death != null)
            death.Dead();
    }

    public void SetDir(Vector2 dir) // Método invocable que recibe la dirección que debe seguir la bala
    {
        transform.parent = null;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = dir.normalized * speed;
    }

}
