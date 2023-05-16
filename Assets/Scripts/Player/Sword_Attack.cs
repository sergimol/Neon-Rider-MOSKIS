using UnityEngine;

public class Sword_Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("he entrado");
        //Cacheamos que el GameObject que entre tenga el componente asociado
        //Guardamos el componente en enemy
        Enemy_Death enemy;
        enemy = other.gameObject.GetComponent<Enemy_Death>();
        DestructibleWall wall;
        wall = other.gameObject.GetComponent<DestructibleWall>();
        BossCrystal crystal;
        crystal = other.gameObject.GetComponent<BossCrystal>();
        Ralentizador ralen;
        ralen = other.gameObject.GetComponent<Ralentizador>();
        Turret turret;
        turret = other.gameObject.GetComponent<Turret>();
        BossBehaviour boss;
        boss = other.gameObject.GetComponent<BossBehaviour>();

        //Si el GameObject tiene el componente, llama al metodo de dicho componente
        if (enemy != null && enabled && enemy.enabled)
        {
            if (turret != null || ralen != null)
                AudioManager.instance.Play(AudioManager.ESounds.RalenTurretDeath);
            enemy.OnAttack();
            Debug.Log("verga");
        }

        //Si tiene el componente DestructibleWall, llama a su método
        else if(wall != null && enabled)
        {
            AudioManager.instance.Play(AudioManager.ESounds.RompePared);
            wall.Break();
        }

        else if (crystal != null && enabled)
        {
            crystal.Break();
        }

        else if (boss != null && enabled)
        {
            boss.OnAttack();
        }
    }
}
