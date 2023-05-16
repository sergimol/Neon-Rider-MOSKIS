using UnityEngine;

// Asociado a la bala. Toma la posición del jugador y se mueve hacia ella con una velocidad constante

public class TurretAttack : MonoBehaviour
{
    int numBullets = 3; // Número de balas esperadas, puede ser menor
    [SerializeField] GameObject bulletPrefab = null;
    public void TurAttack(Transform player) // Método invocable de disparo
    {
        Vector2 dir = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y), dirAux = dir; // Línea recta hacia el jugador
        
        if (bulletPrefab != null)
            for(int i = 0; i < numBullets; i++){ // Alteración de esa línea recta para dar efecto de dispersión
                switch (i)
                {
                    case 0:
                        dir.x-=16;
                        break;
                    case 1:
                        //dir.x++;
                        break;
                    case 2:
                        dir.x+= 16;
                        break;
                    case 3:
                        dir.y-=16;
                        break;
                    case 4:
                        dir.y+= 16 ;
                        break;
                }

                Instantiate(bulletPrefab, transform);
                GetComponentInChildren<TurretBullet>().SetDir(dir);
                dir = dirAux;
            }
    }
}
