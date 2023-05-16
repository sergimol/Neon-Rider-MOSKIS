using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDeathBossSpawn : MonoBehaviour
{
    [SerializeField]
    BossBehaviour boss = null;
    [SerializeField]
    int val = 1;
    private void Start()
    {
        if (GameManager.instance.deadVal != val)
           Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(boss != null && other.GetComponent<PlayerController>() != null){
            if(GameManager.instance.deadVal == val){
                boss.FirstInstance();
                Destroy(this.gameObject);
            }
        }
    }
}
