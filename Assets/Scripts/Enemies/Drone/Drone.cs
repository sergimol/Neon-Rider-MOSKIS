using UnityEngine;
// *ENEMIGO*
// Una vez entra el jugador en su rango de visión, le persigue hasta que logra acercarse lo suficiente
// Acto seguido, y tras un breve periodo de tiempo, explota

public class Drone : MonoBehaviour
{
    [SerializeField] Transform playerT = null;
    [SerializeField] GameObject explosion = null;
    [SerializeField] float speed = 10f;
    bool onRange = false;
    [SerializeField] float time = 0.6f;
    Vector2 direction;
    Rigidbody2D rb;
    EnemyVision vision;
    
    void Start()
    {
        vision = GetComponent<EnemyVision>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (onRange) // La variable onRange controla si el dron tiene que preparase para explotar o no
        {

            direction = Vector2.zero;
            if (time > 0)
                time -= Time.deltaTime;
            if (time <= 0)
            {
                Debug.Log("Pum");
                Instantiate(explosion, transform.position, Quaternion.identity, null); // Al explotar hace aparecer un gameObject que representa la explosión
                AudioManager.instance.Play(AudioManager.ESounds.DronExp);
                Destroy(this.gameObject);
            }
        }

        else if (!onRange) // En caso de no estar en rango, sigue persiguiendo al jugador
        {
            if (vision.Spotted(playerT))
            {
                direction = new Vector2(playerT.position.x - transform.position.x, playerT.position.y - transform.position.y);
                direction.Normalize();
            }
            else
                direction = Vector2.zero;
        }


    }

    private void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision) // Al detectar colisión del jugador cambia onRange a true
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null){
            onRange = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Congela la posición para que no se pueda empujarlo antes de que explote
        }
    }

   
}
