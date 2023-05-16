using UnityEngine;

// En base a las posiciones recibidas del editor (gameObject vacíos hijos de la torreta), va posicionándose entre ellas, con una pausa entre medias
// en la que busca y dispara al jugador en caso de encontrarlo
// Además, mientras se mueve es invulnerable



[RequireComponent(typeof(EnemyVision))]
[RequireComponent(typeof(TurretAttack))]
[RequireComponent(typeof(Rigidbody2D))]

public class Turret : MonoBehaviour
{
    [SerializeField] float speed = 1f, moveIni = 0f, changePos = 2.5f, cadence = 1f;
    [SerializeField] Transform player = null;
    [SerializeField] Transform[] children = null;
    int nextPos;
    float cadenceAux = 0;
    bool comeback = false;
    TurretAttack attack;
    EnemyVision vision;
    Enemy_Death death;
    Animator animator;
    Transform child;
    int childcount;
    AnimatorStateInfo animState;

    void Awake()
    {
        vision = GetComponent<EnemyVision>();
        attack = GetComponent<TurretAttack>();
        death = GetComponent<Enemy_Death>();
        childcount = transform.childCount;
    }

    private void Start()
    {
        int i = 0;
        while(i<children.Length)
        {
            children[i].SetParent(null);
            i++;
        }
        child = transform.GetChild(0);
        animator = child.GetComponent<Animator>();
        nextPos = 1; //Primera posicion 
    }


    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, children[nextPos].position, speed * Time.deltaTime); //Movimiento hacia el punto designado

        if (player != null)
        {
            if (Vector2.Distance(transform.position, children[nextPos].position) < 0.2f) //Si está en posición
            {
                if (moveIni <= 0) //Fase de cambio de posición
                {
                    moveIni = changePos;
                    GetNextPos();
                    cadenceAux = cadence;
                }
                else //Fase de ataque
                {
                    death.enabled = false;
                    moveIni -= Time.deltaTime;
                    animator.SetBool("Moving", false);

                    if (vision.Spotted(player))
                    {
                        if (cadenceAux <= 0){
                            AudioManager.instance.Play(AudioManager.ESounds.TurretShot);
                            attack.TurAttack(player);
                            cadenceAux = cadence;
                        }
                        else{
                            cadenceAux -= Time.deltaTime;
                        }
                    }
                }
            }
            else //Si se está moviendo
            {
                if(vision.Spotted(player))
                    AudioManager.instance.Play(AudioManager.ESounds.TurretWalk);
                animator.SetBool("Moving", true);
                death.enabled = true;
                attack.enabled = false;
            }
        }
    }

    void GetNextPos() // Cálculo de la siguiente posición de la torreta
    {
        if (comeback)
        {
            if (nextPos > 0){
                nextPos--;
            }
            else{
                comeback = false;
                nextPos = 1;
            }
        }
        else
        {
            if (nextPos < children.Length - 1){
                nextPos++;
            }
            else{
                comeback = true;
                nextPos = children.Length - 2;
            }
        }
    }
}