using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelTrigger : MonoBehaviour
{
    [SerializeField]
    int level = 1;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            Tracker.instance.AddEvent(new InicioNivelEvent(level));
            Destroy(this);
        }
    }
}
