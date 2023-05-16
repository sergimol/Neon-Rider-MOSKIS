using UnityEngine;
using UnityEngine.SceneManagement;

public class Activada : MonoBehaviour
{
    [SerializeField] float activateTime = 10f;
    [SerializeField] float deactivateTime = 10f;
    int cont = 0;
    double time;
    Animator animator;
    Transform child;
    new Collider2D collider;
    AnimatorStateInfo animState;
    bool shooting = false;
    float clock = 0;
    bool first = true;
    //Trampa que se activa al ser pisada, tras unos segundos hace daño, y tras unos segundos se vueve a 
    //desactivar.

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        child = transform.GetChild(0);
        animator = child.GetComponent<Animator>();
    }
    void Update()
    {
        clock += Time.deltaTime;
        animState = animator.GetCurrentAnimatorStateInfo(0);
        //empieza el tiempo de activacion tras pisarla
        if (cont == 1)
        {
            time = activateTime;
            clock = 0;
            cont = 0;
            first = false;
        }
        //transcurrido el tiempo 
        if (clock >= time && !first)
        {
            //se activa si esta desactivada e inicia el tiempo hasta desactivarse
            if (!shooting)
            {
                shooting = true;
                time = deactivateTime;
                clock = 0;
                animator.SetBool("act", false);
            }
            //se desactiva y queda en reposo hasta pisarla de nuevo
            else if (shooting)
            {
                shooting = false;
                first = true;
            }
           
        }
    }

    //Esta desactivada, al entrar en contacto empieza a activarse
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Death>()!=null)
        {
            cont = 1;
            animator.SetBool("act", true);
        }
    }

    //si esta en fase de hacer daño, mata tanto al jugador como enemigos que entren en contacto
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
