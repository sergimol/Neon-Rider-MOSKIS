using UnityEngine;

// Comienza el diálogo cuando el jugador entra en el trigger del checkpoint 

public class DialogueTrigger : MonoBehaviour
{
    DialogueManager diaMan;
    int checkpointDeadVal;
    [SerializeField] bool escalera = false;

    void Start()
    {
        diaMan = GetComponent<DialogueManager>();
        Checkpoint checkpoint = GetComponent<Checkpoint>();
        if (checkpoint != null)
            checkpointDeadVal = checkpoint.deadVal;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Comienza el diálogo si no ha pasado todavía por el checkpoint que lo contiene
        if (collision.GetComponent<PlayerController>() != null && GameManager.instance.deadVal != checkpointDeadVal)
        {
            Debug.Log("entró");
            diaMan.StartDialogue();
            this.enabled = false;
            if (escalera) // Si es una escalera, envía lejos al player para que no interfiera con la escena final de cada nivel
            {
                Vector3 pos = new Vector3(50000, 1, 0);
                collision.transform.position = pos;
            }
        }
    }
}
