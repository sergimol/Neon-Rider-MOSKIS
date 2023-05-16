using UnityEngine;

// Asociado al enemigo. Instancia un objeto "bullet" con un tiempo entre invocaciones dado por la variable "cadencia"

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject bullet = null;
    [SerializeField] float cadencia = 2;
    EnemyVision vision;
    public Transform player;
    AnimatorStateInfo animState;
    Drone drone;
    Transform child, rotator;
    float fire = 0;
    Animator animator, animatorArm;
    EnemyMovement movement;
    FlasherMovement flasher;
    bool shooting;

    private void Start()
    {
        flasher = GetComponent<FlasherMovement>();
        movement = GetComponent<EnemyMovement>();
        drone = GetComponent<Drone>();
        vision = GetComponent<EnemyVision>();

        if (drone == null){
            child = transform.GetChild(0);
            animator = child.GetComponent<Animator>();
            animatorArm = child.GetChild(0).GetComponentInChildren<Animator>();
            rotator = child.GetChild(0);
        }            
    }

    void Update()
    {
        animState = animator.GetCurrentAnimatorStateInfo(0);
        if (flasher!=null)
            shooting = animState.IsName("FlasherBodyShot");
        else if (movement!=null)
            shooting = animState.IsName("Maton_Shooting");
        

        // Cuando "Time.time" alcanza el nuevo valor de "firstFire", Instancia un objeto 
        // "bullet" en la posición del enemigo y aumenta el valor de "firstFire" mediante
        // la variable "cadencia"

        if (transform != null && player != null)
        {
            fire += Time.deltaTime;
            // Diferencia entre los 2 vectores - player y DoubleBullet
            Vector3 difference = player.position - transform.position;
            // Devuelve el ángulo cuya tangente es y/x y lo aplica a la rotación del objeto
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;            
            if (player.position.x > transform.position.x){
                transform.localScale = new Vector3(-1f, 1f, 1);
                rotator.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }               
            else
            {
                transform.localScale = new Vector3(1f, 1f, 1);
                rotator.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 180);
            }
            if (fire > cadencia ){
                if(flasher != null)
                    AudioManager.instance.Play(AudioManager.ESounds.FlasherRay);
                Instantiate(bullet, child.GetChild(0).GetChild(0).GetChild(0).position, Quaternion.identity, transform);
                fire = 0;
            }
            else if (fire >cadencia - 0.32 && vision.Spotted(player) && !shooting)
            {
                SetAnimator(true);
            }
            else if (shooting){
                SetAnimator(false);
            }
            else if (!vision.Spotted(player)){
                SetAnimator(false);
                fire = 0;
            }
        }            
    }

    void SetAnimator(bool shoot)
    {
        animator.SetBool("Shooting", shoot);
        animatorArm.SetBool("Shooting", shoot);
    }
}
