using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnControl : MonoBehaviour
{
    // Script auxiliar a BossBehaviour colocado en los enemigos a invocar
    // Al morir, notifica de su muerte al Boss


    [SerializeField]
    BossBehaviour fatherBoss = null;
    [SerializeField]
    PlayerController player = null;
    
    private void OnDisable()
    {
        if (player != null && fatherBoss != null)
        {
            fatherBoss.UpdateEnemies(transform);
        }
    }
}
