using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que controla las acciones del personaje
public class Personaje : MonoBehaviour
{
    Vector2 salto, caida;

    [Header("Variables")]
    public float fuerzaSalto = 2.0f;
    public float fuerzaCaida = 3.0f;
    public float fuerzaMorir = 10f;
    public float tiempoCaida = 0.1f; //Tiempo entre que salta hasta que se permite bajar
    public float pesoGato = 2f;
    public float tiempoTrasMorir = 3f;//Tiempo desde que muere hasta que aparece la pantalla de gameOver


    public ManagerPartida mp;
    float contadorMuerte;
    public bool estaSuelo, estaAire, estaCayendo, estaVivo;
    Rigidbody2D rb;
    Animator at;
    public AudioClip[] audioSalto;
    public AudioClip[] audioCaida;
    AudioSource audioS;
    Renderer text;

    void Start()
    {
        mp = GameObject.Find("GameManager").GetComponent<ManagerPartida>();
        rb = GetComponent<Rigidbody2D>();
        salto = new Vector3(0.0f, 2.0f);
        caida = new Vector3(0.0f, -2.0f);
        estaVivo = true;
        contadorMuerte = tiempoTrasMorir;
        GetAnimator();
        audioS = GetComponent<AudioSource>();
        estaSuelo = true;
        estaAire = false;
        estaCayendo = false;
    }

    //Al tocar suelo
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "suelo" && (mp.jugando || mp.EsTutorial()))
        {
            estaSuelo = true;
            estaAire = false;
            estaCayendo = false;
            PonerAnimacion(estaSuelo, estaAire, estaCayendo);
        }
    }

    public void GetAnimator() //Variable para coger el animato de la skin actual
    {
        at = transform.GetComponentInChildren<Animator>();
    }

    public void GetMaterial() //Variable para coger el animato de la skin actual
    {
        text = transform.GetChild(0).GetComponentInChildren<Renderer>();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Saltar(false);
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Saltar(false);
            }
        }

        if (!estaSuelo && estaVivo)
        {
            Vector3 vel = rb.velocity;
            vel.y -= pesoGato * Time.deltaTime;
            rb.velocity = vel;
        }
        else if (!estaVivo && mp.jugando)
        {
            contadorMuerte -= Time.deltaTime;
            if (contadorMuerte <= 0)
            {
                mp.GameOver();
            }
        }
        
    }

    //Pone la animacion que toque. Esta funcion se llama al saltar, golpear o tocar el suelo
    void PonerAnimacion(bool suelo, bool aire, bool cayendo)
    {
        if(aire)
        {
            at.SetInteger("NumAnim", Random.Range(1, 4)); 
        }
        at.SetBool("Suelo", suelo);
        at.SetBool("Aire", aire);
        at.SetBool("Cayendo", cayendo);
    }

    //Empuja al jugador en la direccion del proyectil
    public void Muerte(Vector2 puntoColision, Vector2 direccion)
    {
        if (estaVivo)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.AddForceAtPosition(direccion * fuerzaMorir, puntoColision);
            estaVivo = false;
            RecibirDaño();
        }
    }

    //Si esta en el suelo salta, si esta en el aire baja
    public void Saltar(bool excepcion)
    {
        if((estaVivo && mp.jugando) || excepcion)
        {
            if (estaSuelo)
            {
                SonidoSalto(true);
                rb.AddForce(salto * fuerzaSalto, ForceMode2D.Impulse);
                estaSuelo = false;
                PonerAnimacion(false,true,false);
                StartCoroutine(PermitirBajar());
            }
            else if (estaAire)
            {
                SonidoSalto(false);
                estaCayendo = true;
                rb.AddForce(caida * fuerzaCaida, ForceMode2D.Impulse);
                PonerAnimacion(false,false,true);
                estaAire = false;
            }
        }
    }

    void SonidoSalto(bool saltar)
    {
        int n = Random.Range(0, 6);
        if (saltar) audioS.clip = audioSalto[n];
        else audioS.clip = audioCaida[n];

        audioS.Play();
    }

    //Renicio las variables del jugador
    public void Reniciar() 
    {
        contadorMuerte = tiempoTrasMorir;
        estaVivo = estaSuelo =  true;
        estaAire = estaCayendo = false;
        PonerAnimacion(estaSuelo, estaAire, estaCayendo);

        if(rb.isKinematic)rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        transform.rotation = new Quaternion(0, 0, 0, 0); 

    }

    //Permite al jugador bajar despues de un tiempo
    IEnumerator PermitirBajar()
    {
        yield return new WaitForSeconds(tiempoCaida);
        estaAire = true;
    }

    public void RecibirDaño()
    {
        text.material.SetColor("_Color", new Color(0.9339623f, 0.1806248f, 0.1806248f));
        StartCoroutine(PonerColorNormal());
    }

    IEnumerator PonerColorNormal()
    {
        yield return new WaitForSeconds(0.2f);
        text.material.SetColor("_Color", new Color(1,1,1));
    }
}
