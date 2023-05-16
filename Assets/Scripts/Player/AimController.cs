using UnityEngine;
using UnityEngine.Audio;

public class AimController : MonoBehaviour
{
    Animator[] anim;
    SpriteRenderer[] neon;
    int attackIndicator;
    AnimatorStateInfo animState;
    CircleCollider2D attackCollider;
    CapsuleCollider2D blockCollider;
    Sprite playerSpr;
    bool attackBool, blockBool;
    Bloqueo parry;

    private void Awake()
    {
        //anim[0] player //anim[1] sword
        anim = GetComponentsInChildren<Animator>();
        neon = GetComponentsInChildren<SpriteRenderer>();
        //neon[1].color = Color.red;
        attackCollider = GetComponent<CircleCollider2D>();
        blockCollider = GetComponent<CapsuleCollider2D>();
        parry = GetComponent<Bloqueo>();
    }

    void Update()
    {
        //Cogemos el estado de la animacion en cada frame
        animState = anim[0].GetCurrentAnimatorStateInfo(0);
        //Hacemos que el booleano se active solo cuando se activa la animación
        attackBool = animState.IsName("Player_Attack");
        blockBool = animState.IsName("Player_Block");
        playerSpr = GetComponent<SpriteRenderer>().sprite;
        //63 left, 15 down, 34 up, 70 right
        if ((playerSpr.name == "Player_70" || playerSpr.name == "Player_63" || playerSpr.name == "Player_15" || playerSpr.name == "Player_34") && !blockBool){
            attackCollider.enabled = true;
        }
        else{
            attackCollider.enabled = false;
        }

        Vector2 mov;

        // Mientras no estén activas las animaciones de ataque o bloqueo, el 
        // jugador apunta en la dirección en la que se mueve
        if (!attackBool && !blockBool)
        {
            if (GameManager.instance.mando)
            {
                mov = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            else
            {
                Vector2 direccionx, direcciony;
                if (Input.GetKey("w")) direcciony = new Vector2(0, 1);
                else if (Input.GetKey("s")) direcciony = new Vector2(0, -1);
                else direcciony = new Vector2(0, 0);

                if (Input.GetKey("d")) direccionx = new Vector2(1, 0);
                else if (Input.GetKey("a")) direccionx = new Vector2(-1, 0);
                else direccionx = new Vector2(0, 0);

                mov = direccionx + direcciony;
            }
            // Si se mueve el joystick en el eje horizontal:
            if (Mathf.Abs(mov.x) >= Mathf.Abs(mov.y) && mov != Vector2.zero && !GameManager.instance.gameIsPaused)
            {
                anim[0].SetFloat("PosY", 0);
                anim[1].SetFloat("PosY", 0);
                if (mov.x >= 0) // Mirar Derecha
                {
                    anim[0].SetFloat("PosX", 1);
                    anim[1].SetFloat("PosX", 1);
                    attackIndicator = 1;
                }
                else // Mirar Izquierda
                {
                    anim[0].SetFloat("PosX", -1);
                    anim[1].SetFloat("PosX", -1);
                    attackIndicator = 2;
                }
            }

            else if (mov != Vector2.zero && !GameManager.instance.gameIsPaused)
            {
                anim[0].SetFloat("PosX", 0);
                anim[1].SetFloat("PosX", 0);
                if (mov.y >= 0) // Mirar Arriba
                {
                    anim[0].SetFloat("PosY", 1);
                    anim[1].SetFloat("PosY", 1);
                    attackIndicator = 3;

                }
                else // Mirar Abajo
                {
                    anim[0].SetFloat("PosY", -1);
                    anim[1].SetFloat("PosY", -1);
                    attackIndicator = 4;
                    //Debug.Log(indicadorAtaque);

                }
            }
        }

        //ATAQUE 
        if ((Input.GetKeyDown("joystick button 5") || Input.GetMouseButtonDown(0)) && !attackBool && !GameManager.instance.gameIsPaused)
        {
            parry.enabled = false;
            GetComponent<Sword_Attack>().enabled = true;
            Attack(attackIndicator, ref attackCollider, anim);
        }

        //BLOQUEO
        else if ((Input.GetKeyDown("joystick button 4") || Input.GetMouseButtonDown(1)) && !attackBool && !blockBool && !GameManager.instance.gameIsPaused) //BLOQUEO
        {

            mov = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            // Si se mueve el joystick en el eje horizontal:
            if (Mathf.Abs(mov.x) >= Mathf.Abs(mov.y) && mov != Vector2.zero && !GameManager.instance.gameIsPaused)
            {
                anim[0].SetFloat("PosY", 0);
                anim[1].SetFloat("PosY", 0);
                if (mov.x >= 0) // Mirar Derecha
                {
                    anim[0].SetFloat("PosX", 1);
                    anim[1].SetFloat("PosX", 1);
                    attackIndicator = 1;
                }
                else // Mirar Izquierda
                {
                    anim[0].SetFloat("PosX", -1);
                    anim[1].SetFloat("PosX", -1);
                    attackIndicator = 2;
                }
            }

            else if (mov != Vector2.zero && !GameManager.instance.gameIsPaused)
            {
                anim[0].SetFloat("PosX", 0);
                anim[1].SetFloat("PosX", 0);
                if (mov.y >= 0) // Mirar Arriba
                {
                    anim[0].SetFloat("PosY", 1);
                    anim[1].SetFloat("PosY", 1);
                    attackIndicator = 3;

                }
                else // Mirar Abajo
                {
                    anim[0].SetFloat("PosY", -1);
                    anim[1].SetFloat("PosY", -1);
                    attackIndicator = 4;
                    //Debug.Log(indicadorAtaque);

                }
            }

            GetComponent<Sword_Attack>().enabled = false;
            anim[0].SetTrigger("Block");
            anim[1].SetTrigger("Block");
            switch (attackIndicator)
            {
                case 1:  //Block Right
                    blockCollider.direction = (CapsuleDirection2D)0;
                    blockCollider.offset = new Vector2(0.18f, 0.03f);
                    blockCollider.size = new Vector2(0.48f, 0.84f);
                    break;
                case 2:   //Block Left
                    blockCollider.direction = (CapsuleDirection2D)0;
                    blockCollider.offset = new Vector2(-0.18f, 0.03f);
                    blockCollider.size = new Vector2(0.48f, 0.84f);
                    break;
                case 3:   //Block Up
                    blockCollider.direction = (CapsuleDirection2D)1;
                    blockCollider.offset = new Vector2(-0.05f, 0.18f);
                    blockCollider.size = new Vector2(0.99f, 0.48f);
                    break;
                case 4:   //Block Down
                    blockCollider.direction = (CapsuleDirection2D)1;
                    blockCollider.offset = new Vector2(0.07f, -0.18f);
                    blockCollider.size = new Vector2(0.99f, 0.48f);
                    break;
            }
            parry.enabled = true;
        }
    }

    static void Attack(int attackIndicator, ref CircleCollider2D attackCollider, Animator[] anim)
    {
        //attackCollider.enabled = true;
        Vector2 mov = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Si se mueve el joystick en el eje horizontal:
        if (Mathf.Abs(mov.x) >= Mathf.Abs(mov.y) && mov != Vector2.zero && !GameManager.instance.gameIsPaused)
        {
            anim[0].SetFloat("PosY", 0);
            anim[1].SetFloat("PosY", 0);
            if (mov.x >= 0) // Mirar Derecha
            {
                anim[0].SetFloat("PosX", 1);
                anim[1].SetFloat("PosX", 1);
                attackIndicator = 1;
            }
            else // Mirar Izquierda
            {
                anim[0].SetFloat("PosX", -1);
                anim[1].SetFloat("PosX", -1);
                attackIndicator = 2;
            }
        }

        else if (mov != Vector2.zero && !GameManager.instance.gameIsPaused)
        {
            anim[0].SetFloat("PosX", 0);
            anim[1].SetFloat("PosX", 0);
            if (mov.y >= 0) // Mirar Arriba
            {
                anim[0].SetFloat("PosY", 1);
                anim[1].SetFloat("PosY", 1);
                attackIndicator = 3;

            }
            else // Mirar Abajo
            {
                anim[0].SetFloat("PosY", -1);
                anim[1].SetFloat("PosY", -1);
                attackIndicator = 4;
                //Debug.Log(indicadorAtaque);

            }
        }

        anim[0].SetTrigger("Attack");
        anim[1].SetTrigger("Attack");
        AudioManager.instance.Play(AudioManager.ESounds.Swing); // Hace que suene el sonido asociado al ataque
        switch (attackIndicator)
        {
            case 1:   //Right Attack
                attackCollider.offset = new Vector2(0.3f, 0.12f);
                attackCollider.radius = 0.4f;
                break;
            case 2:   //Left Attack
                attackCollider.offset = new Vector2(-0.3f, 0.12f);
                attackCollider.radius = 0.4f;
                break;
            case 3:   //Up Attack
                attackCollider.offset = new Vector2(-0.07f, 0.22f);
                attackCollider.radius = 0.42f;
                break;
            case 4:   //Down Attack
                attackCollider.offset = new Vector2(0, -0.3f);
                attackCollider.radius = 0.42f;
                break;
        }
    }
}

