using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Maneja el movimiento en las 4 direcciones
//Tambien acciona las animaciones correspondiente a cada direccion
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 15;
    Vector2 direccion;
    Animator[] anim;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //anim[0] player //anim[1] sword
        anim = GetComponentsInChildren<Animator>();
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 direccionx;
        Vector2 direcciony;
        if (GameManager.instance.mando) // MANDO
        {
            direccion = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            direccion.Normalize();
        }
        else // TECLADO Y RATÓN
        {
            if (Input.GetKey("w")) direcciony = new Vector2(0, 1);
            else if (Input.GetKey("s")) direcciony = new Vector2(0, -1);
            else direcciony = new Vector2(0, 0);

            if (Input.GetKey("d")) direccionx = new Vector2(1, 0);
            else if (Input.GetKey("a")) direccionx = new Vector2(-1, 0);
            else direccionx = new Vector2(0, 0);

            direccion = direccionx + direcciony;
            direccion.Normalize();
        }
        anim[0].SetFloat("Speed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
        anim[1].SetFloat("Speed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y));
    }

    private void FixedUpdate()
    {
        rb.velocity = direccion * speed;
    }

    //Metodos usados en los PowerUps Verde y Azul para manejar la velocidad del jugador
    public void MulSpeed(int x)
    {
        speed *= x;
    }
    public void DivSpeedReset(int x)
    {
        speed /= x;
    }
   
}
