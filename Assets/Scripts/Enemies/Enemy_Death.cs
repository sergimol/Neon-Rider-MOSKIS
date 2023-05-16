using UnityEngine;

public class Enemy_Death : MonoBehaviour
{
    public int hitsToDeath; // Golpes que aguanta el enemigo
    [SerializeField] int deadVal = 0;
    Transform child;
    EnemyVision enemy;
    Drone drone;
    Turret turret;
    PrestEnemyAttack prest;
    Ralentizador ralen;
    [SerializeField] int id;

    private void Start()
    {
        // Si se reanuda desde un checkpoint posicionado después del enemigo,
        // éste es destruido
        if (GameManager.instance.deadVal >= deadVal){
            Destroy(this.gameObject);
        }

        prest = GetComponent<PrestEnemyAttack>();
        enemy = GetComponent<EnemyVision>();
        drone = GetComponent<Drone>();
        turret = GetComponent<Turret>();
        ralen = GetComponent<Ralentizador>();

        // Si no es el ralentizador, drone o torreta, cogemos al hijo
        if ((enemy != null||ralen!=null) && drone == null && turret == null)
            child = transform.GetChild(0);
        else if (turret != null && ralen==null && drone==null)
        {
            int i = 0;
            bool sprite = false;
            while (!sprite && transform.GetChild(i) != null)
            {
                if(transform.GetChild(i).GetComponent<Animator>()!=null)
                {
                    child = transform.GetChild(i);
                    sprite = true;
                }
                i++;    
            }
        }
    }
    // Metodo llamado desde el componente Sword_Attack
    public void OnAttack()
    {
        if (drone == null)
        {
            hitsToDeath--; // Recibe daño
            if (turret == null && ralen == null)
                AudioManager.instance.Play(AudioManager.ESounds.Hit); // Sonido de daño del matón
            else
            {
                AudioManager.instance.Stop(AudioManager.ESounds.RalentTime);
                AudioManager.instance.Play(AudioManager.ESounds.RalenTurretDeath);
            }
        }
        
   
        if (hitsToDeath == 0)
        {
            if ((enemy != null || ralen != null || prest != null || turret != null) && drone == null)
            {
                // Separamos al hijo del padre
                // Llamamos a Animation de "EnemyDeathAnim"
                if (child.GetComponent<Animator>() != null)
                {
                    child.GetComponent<Animator>().SetBool("Death", true);
                    if (prest == null && ralen == null && turret==null)
                        child.GetChild(0).GetComponentInChildren<SpriteRenderer>().enabled = false;
                    child.SetParent(null);
                }
            }
            Tracker.instance.AddEvent(new MuerteEnemigoEvent(transform.position, id));
            Destroy(this.gameObject);
        }
    }

    public int getId()
    {
        return id;
    }
}