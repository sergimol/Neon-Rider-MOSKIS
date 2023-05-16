using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]

/*
*   Componente del jugador
*   Activa el CircleCollider del jugador al recibir la orden desde el PlayerController
*/
public class Bloqueo : MonoBehaviour
{


    CapsuleCollider2D collisionArea;
    [SerializeField] float blockTime = 0.1f;
    float blocking;
    ActivatePowerUpRed activPow;
    ActivatePowerUpPurple activPowPurple;
    PowerUpPurple purple;
    float time;

    private void Awake()
    {
        collisionArea = this.GetComponent<CapsuleCollider2D>();
        activPow = GetComponent<ActivatePowerUpRed>();
        activPowPurple = GetComponent<ActivatePowerUpPurple>();
        purple = GetComponent<PowerUpPurple>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (blockTime <= time)
            enabled = false;
    }

    private void OnEnable()
    {
        collisionArea.enabled = true;
        time = 0;

    }

    private void OnDisable()
    {
        collisionArea.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled)
        {
            if (collision.GetComponent<Bullet>() != null) // Caso powerup Rojo
            {
                if (!purple.enabled)
                {
                    Destroy(collision.gameObject);
                }
                activPow.AddToCont();
                Tracker.instance.AddEvent(new BloqueoEvent(transform.position)) ;
                Debug.Log("Parryada");
            }
            if (collision.GetComponent<PrestBullet>() != null && purple.enabled == false) 
            {
                AudioManager.instance.Play(AudioManager.ESounds.Bloqueo3);
                Destroy(collision.gameObject);
                Debug.Log("BALA DESTRUIDA PREST");

            }
            if (collision.GetComponent<TurretBullet>() != null && purple.enabled == false) 
            {
                AudioManager.instance.Play(AudioManager.ESounds.Bloqueo3);
                Destroy(collision.gameObject);
                Debug.Log("BALA DESTRUIDA TURRET");

            }
        }
    }
}

