using UnityEngine;
//Potenciador Morado
public class PowerUpPurple : MonoBehaviour
{
    //No necesita ningun componente extra
    PrestBullet purpleBulletComponent;
    Bullet redBulletComponent;
    TurretBullet turretBulletComponent;
    Bloqueo parry;
    Rigidbody2D rbBullet;
    GameObject bullet;
    Vector2 auxVel=new Vector2(2,0);
    GameObject chid;
    void OnTriggerEnter2D(Collider2D collision)
    {
        parry = GetComponent<Bloqueo>();
        //Cogemos la referencia de las balas que entren al trigger de bloqueo
        //Identificacion
        purpleBulletComponent = collision.gameObject.GetComponent<PrestBullet>();
        redBulletComponent = collision.gameObject.GetComponent<Bullet>();
        turretBulletComponent = collision.gameObject.GetComponent<TurretBullet>();
        //GameObject para cambiar de layer
        bullet = collision.gameObject;
        //Rb para cambiar el sentido
        rbBullet = collision.gameObject.GetComponent<Rigidbody2D>();
    
        //Si el bloqueo esta activado devolvemos las balas (cambiamos el sentido y la layer para que impacten contra los enemigos)
        if ((purpleBulletComponent != null|| redBulletComponent != null || turretBulletComponent != null) && parry.enabled )
        {
            rbBullet.velocity = -rbBullet.velocity;
            bullet.layer = 13;               
        }
                          
    }
}
