using UnityEngine;

public class FlasherMovement : MonoBehaviour
{
    [SerializeField] Transform player = null;
    new Renderer renderer;
    //AnimatorStateInfo estadoAnimacion; SIN USO
    Transform child;
    Animator animator;
    EnemyVision enemy;
    bool ya = true; 

    private void Start()
    { 
        enemy = GetComponent<EnemyVision>();
        if (enemy != null)
        { 
            child = transform.GetChild(0);
            renderer = GetComponentInChildren<Renderer>();
            animator = child.GetComponent<Animator>();
        }
    }
    void Update()
    {
        if (player!=null)
        {
            Vector3 diference = player.position - transform.position; // Guarda la dirreccion de la linea que une flasher-player
            float rotationz = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg; // Calculamos el ángulo            
        }
        
        if (renderer.isVisible && ya)
        {
            animator.SetBool("Jump", true);
            Invoke("Apagar", 1);
        }
    }

    void Apagar()
    {
        animator.SetBool("Jump", false);
        ya = false;
    }
}
