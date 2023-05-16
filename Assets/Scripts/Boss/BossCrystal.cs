using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossCrystal : MonoBehaviour
{
    [SerializeField] BossBehaviour boss = null;
    Animator anim;
    Transform child;
    bool active = false;

    void Start()
    {
        child = transform.GetChild(0);
        anim = child.GetComponent<Animator>();
    }
    public void Break()
    {
        if (active && boss != null)
        {
            active = false;

            boss.UpdateCrystal();

            // Separamos al hijo del padre
            // Llamamos a Animation de "EnemyDeathAnim"
            if (anim != null)
            {
                AudioManager.instance.Play(AudioManager.ESounds.CrystalBreak);
                anim.SetBool("Broken", true);
                child.SetParent(null);
            }
            Destroy(this.gameObject);
        }
    }

    public void SetActive()
    {
        active = true;
        Color temp = GetComponentInChildren<SpriteRenderer>().color;
        temp.a = 255f;
        GetComponentInChildren<SpriteRenderer>().color = temp;    
    }

}
