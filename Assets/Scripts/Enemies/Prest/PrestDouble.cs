using UnityEngine;

// Asociado al objeto vacío DoubleBullet. Hace rotar el objeto y activa los objetos hijos bullet con un intervalo de tiempo entre medias

public class PrestDouble : MonoBehaviour
{
    public Transform player;
    PrestEnemyMovement enemyMov;
    GameObject firstBullet, secondBullet;
    [SerializeField] float secondFire = 10f, firstFire = 7f;
    Transform sprite;
    Animator anim;
    float time = 0;

    void Start()
    {
        enemyMov = GetComponentInParent<PrestEnemyMovement>();
        enemyMov.enabled = false;
        player = GetComponentInParent<PrestEnemyAttack>().player;
        sprite = transform.parent;
        anim = sprite.GetComponent<GetAnim>().GetAnimator();

        // Cogemos las balas por separado
        firstBullet = transform.GetChild(0).gameObject;
        secondBullet = transform.GetChild(1).gameObject;

        // Gestionamos tiempos de disparo
        //firstFire += Time.time;
        secondFire += firstFire;
        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        if (player != null)
        {
            // Diferencia entre los 2 vectores - player y DoubleBullet
            Vector3 difference = player.position - transform.position;
            // Devuelve el ángulo cuya tangente es y/x y lo aplica a la rotación del objeto
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ + 90);
        }

        // Activa el comportamiento de las balas tras un tiempo
        if (time > firstFire && firstBullet != null)
        {
            AudioManager.instance.Play(AudioManager.ESounds.PrestShot);
            anim.SetTrigger("Attack 1");
            firstFire = 100;
            firstBullet.GetComponent<PrestBullet>().enabled = true;  
        }

        // SEgunda bala y activación del movimiento del prestidigitador
        if (time > secondFire && secondBullet != null)
        {
            AudioManager.instance.Play(AudioManager.ESounds.PrestShot);
            anim.SetTrigger("Attack 2");
            secondFire = 100;
            secondBullet.GetComponent<PrestBullet>().enabled = true;
            anim.SetBool("Bullets", false);
            enemyMov.enabled = true;
            Destroy(this.gameObject);
        }
    }
}
