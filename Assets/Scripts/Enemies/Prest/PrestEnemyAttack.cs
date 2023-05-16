using UnityEngine;

// Asociado al enemigo. Instancia un objeto "bullet" con un tiempo entre invocaciones dado por la variable "cadencia"

public class PrestEnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject bullet = null;
    [SerializeField] float cadencia = 2, firstFire = 0.5f;
    EnemyVision vision;
    public Transform player;
    Animator anim;
    float time;

    private void Start()
    {
        vision = GetComponent<EnemyVision>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        time += Time.deltaTime;
        // Cuando "Time.time" alcanza el nuevo valor de "firstFire", Instancia un objeto 
        // "bullet" en la posición del enemigo y aumenta el valor de "firstFire" mediante
        // la variable "cadencia"
        if (transform != null)
            if (time > firstFire && vision.Spotted(player)) //TIME.TIME ------------------------------> TIME.DELTATIME
            {
                Instantiate(bullet, transform.position, Quaternion.identity, transform);
                firstFire = cadencia;
                time = 0;
                anim.SetBool("Bullets", true);
            }
    }
}
