using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que controla a los obstaculos que aparecen de los lados
/*
 * Posibles futuros problemas:
 *      La manera de controlar si el jugador esta encima o de lado no esta 100% comprobado.
 *      
 */

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Obstaculos : MonoBehaviour
{
    public Vector3 direccion = new Vector2(-2f, 0f);
    public float velocidad = 5f;
    public float fuerzaEmpuje = 10f;
    public bool congelado = false;

    bool primeraVez = false;

    Spawner spawn;
    ManagerPartida mg;
    Collider2D col;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawn = GameObject.Find("Spawn").GetComponent<Spawner>();
        mg = GameObject.Find("GameManager").GetComponent<ManagerPartida>();
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!congelado) transform.position += direccion * Time.deltaTime * velocidad;
    }

    //Congela el objeto
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "congelador" && primeraVez) //Si el objeto ha sido convertido en suelo y esta colocado en la torre
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player" && !congelado) //Si toca al jugador y no es un suelo, congela o mata al jugador dependiendo de la altura
        {
            Personaje pj = collision.transform.GetComponent<Personaje>();

            Vector2 point = collision.GetContact(0).point;
            if (transform.GetChild(0).position.y < collision.transform.position.y) //NO ES SEGURO AL 100%
            {
                if (pj.estaCayendo)
                {
                    ItemSuelo();
                }
                else
                {
                    pj.Muerte(point, direccion);
                }
            }
            else
            {
                print("BOOM");
                pj.Muerte(point, direccion);
            }
        }
        else if(collision.transform.tag == "suelo")
        {
            //Si el objeto es suelo y toca otro suelo, se actualiza la altura actual
            if(congelado && !primeraVez)
            {
                primeraVez = true;
                spawn.CambiarAltura(gameObject,0);
                
                if(transform.position.y < 0)
                {
                    mg.ActualizarAltura(0 + col.bounds.size.y / 2,transform.position.y);
                }
                else
                {
                    mg.ActualizarAltura(transform.position.y + col.bounds.size.y / 2, transform.position.y);
                }
            }
        }
    }

    //Convierte el item en parte de la torre
    void ItemSuelo()
    {
        congelado = true;
        rb.isKinematic = false;
        tag = "suelo";
    }
}
