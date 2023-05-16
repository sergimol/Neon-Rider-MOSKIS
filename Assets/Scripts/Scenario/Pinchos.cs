using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] float intermitentTime = 1.5f;
    bool cont = true;
    double time = 0;
    new Collider2D collider;
    Animator animator;
    Transform child;
    bool shooting = false;

    void Awake()
    {
        child = transform.GetChild(0);
        animator = child.GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }
    //Se activa y se desactiva intermitentemente
    void Update()
    {
        time += Time.deltaTime;
        if (cont)
        {
            time = 0;
            cont = false;
        }
        if (time>= intermitentTime) 
        {
            //la activa si esta ddesactivada y la desactiva si esta activada
            //y reinicia el temporizador para activarla o desactivarla de nuevo
            if  (shooting)
            {
                shooting = false;
                animator.SetBool("Active", false);
            }
            else
            {
                shooting = true;
                animator.SetBool("Active", true);
            }
            cont = true;
        }
    }
    //mata al jugador o al enemigo cuando están activadas
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (shooting)
        {
            Enemy_Death dead = collision.gameObject.GetComponent<Enemy_Death>();
            if (dead != null)
            {
                dead.OnAttack();
            }

            Death death = collision.gameObject.GetComponent<Death>();
            if (death != null)
                death.Dead();
        }
    }

}
