using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using UnityEngine.SocialPlatforms;

public class ManagerPartida : MonoBehaviour
{
    private int key = 0;
    int _altura;
    int altura
    {
        get { return _altura ^ key; }
        set
        {
            key = Random.Range(0, int.MaxValue);
            _altura = value ^ key;
        }
    }
    float alturaActual;

    public string store_id = "3052902";
    string video_id = "video";
    string reward_id = "rewardedVideo";

    [Header("Variables")]
    public float multiplicadorAltura = 10;
    public float velocidadInicial = 1;
    public float tiempoEntreSpawn = 1;
    public float velocidadMinima = 2.5f;
    public float velocidadMaxima = 3.5f;
    public float posicionCamaraInicio = 6;
    public float segundosContinuar = 3f;
    public float posicionPersonajeInicio = 0.5f;
    public float margenDron = 1.2f;
    public int anunciosCadaPartida = 4;
    public float difCamara = 5;
    public float incrementoVelocidad = 0.5f;
    public float alturaDificultat = 30;

    [Header("Objetos de la escena")]
    public GameObject camara;
    public GameObject mensajeGameOver;
    public GameObject interfazMenu;
    public GameObject interfazOpciones;
    public Spawner spawn;
    public GameObject record;
    public GameObject dron;
    public GameObject jugador;
    public GameObject limitador;
    public GameObject contadorText;
    public Text textAltura;
    public Toggle desactivarMusica, desactivarSonido;
    public SelectorPersonaje select;
    public Text recordAltura;
    public Text finalrecordAltura;
    public Text finalAltura;
    public Sprite sonidoActivado, sonidoDesactivado;
    public GameObject textoDesbloq;
    public GameObject Menututorial;

    
    [HideInInspector]
    public bool jugando = false;
    bool jugandoDeVerdad = false;

    int alturaMaxima, alturaTotal, muertes, numContinuar;
    public int esquivados;

    float contadorContinuar;
    bool continuar = false;

    float actual, ant;
    float cont = 0;
    float velocidadActual = 1;
    AudioSource aS;

    int nAnuncio = 0;
    Image imagenSonido, imagenMusica;
    int musica, sonido;

    int tutorial = 0;
    bool esTutorial = false;
    bool saltadoTutorial = false;


    [Header("Musica")]

    public AudioSource audioPersonaje,audioMenu;
    public AudioClip musicaJugar, musicaDerrota, sonidoStart;

    public Personaje pj;

    Animator animDesblq;
    // Start is called before the first frame update

    float contadorTutorial = 0;

    private void Awake()
    {
        Input.multiTouchEnabled = false;

    }
    void Start()
    {

        Advertisement.Initialize(store_id, false);

        animDesblq = textoDesbloq.GetComponent<Animator>();
        imagenSonido = desactivarSonido.transform.GetChild(0).GetComponent<Image>();
        imagenMusica = desactivarMusica.transform.GetChild(0).GetComponent<Image>();

        aS = GetComponent<AudioSource>();
        musica = PlayerPrefs.GetInt("musica", 1);
        sonido = PlayerPrefs.GetInt("sonido", 1);

        DesactivarSonido();
        DesactivarMusica();

        desactivarMusica.isOn = musica != 0;
        desactivarSonido.isOn = sonido != 0;

        pj = jugador.GetComponent<Personaje>();

        alturaActual = -1.6f;
        alturaTotal = PlayerPrefs.GetInt("alturaTotal", 0);
        muertes = PlayerPrefs.GetInt("Muertes", 0);
        alturaMaxima = PlayerPrefs.GetInt("altura", 0);
        numContinuar = PlayerPrefs.GetInt("numCont", 0);
        esquivados = PlayerPrefs.GetInt("esq", 0);

        nAnuncio = PlayerPrefs.GetInt("anuncio", 0);

        tutorial = PlayerPrefs.GetInt("tutorial", 0);

        if(nAnuncio >= anunciosCadaPartida)
        {
            PlayerPrefs.SetInt("anuncio", 0);
            nAnuncio = 0;
            print("anuncio");
            if(Advertisement.IsReady(video_id))
            {
                Advertisement.Show(video_id);
            }
        }
        if (alturaMaxima>0) //Pone la linea si la altura es mas grande que 0
        {
            recordAltura.enabled = true;
            recordAltura.text = alturaMaxima.ToString() + " m";
        }

        Social.localUser.Authenticate((bool success) => { });

        velocidadActual = velocidadInicial;

        incrementoVelocidad += velocidadInicial;
    }


    // Update is called once per frame
    void Update()
    {
        if (continuar) //Contador para continuar
        {
            contadorContinuar -= Time.deltaTime;
            actual = Mathf.Clamp(Mathf.Round(contadorContinuar), 1, segundosContinuar);
            if(actual!=ant)
            {
                audioMenu.Play();
                contadorText.GetComponent<Text>().text = actual.ToString();
                ant = actual;
            }

            if (contadorContinuar<=0)
            {
                continuar = false;
                contadorText.SetActive(false);
                InicioPartida();
            }
        }
        else if (jugando) //Tiempo entre spawns de objetos y subir la camara
        {
            cont -= Time.deltaTime;
            if (cont <= 0)
            {
                cont = tiempoEntreSpawn;
                spawn.Spawn(Random.Range(velocidadMinima,velocidadMaxima));
            }

            if(jugandoDeVerdad)
            {
                if (camara.transform.position.y - alturaActual < difCamara)
                {
                    velocidadActual = incrementoVelocidad;
                }
                else
                {
                    velocidadActual = velocidadInicial;
                }
                camara.transform.Translate(Vector3.up * Time.deltaTime * velocidadActual);
            }
        }
        else if(esTutorial)
        {
            switch (tutorial)
            {
                case 0:
                    Tutorial1();
                    break;
                case 1:
                    Tutorial2();
                    break;
                case 2:
                    Tutorial3();
                    break;
                case 3:
                    Tutorial4();
                    break;
            }
        }
    }

    void Tutorial1()
    {
        if(contadorTutorial < 3)
        {
            contadorTutorial += Time.deltaTime;
        }
        else
        {
            spawn.tutorial = true;
            spawn.Spawn((velocidadMinima+velocidadMaxima)/2);
            contadorTutorial = 0;
            tutorial = 1;
        }
    }

    void Tutorial2()
    {
        if (contadorTutorial < 0.7)
        {
            contadorTutorial += Time.deltaTime;
        }
        else
        {
            Time.timeScale = 0;
            Menututorial.SetActive(true);
            Menututorial.transform.GetChild(0).gameObject.SetActive(true);
            contadorTutorial = 0;
            tutorial = 2;
        }
    }

    void Tutorial3()
    {
        if (!saltadoTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                Menututorial.transform.GetChild(0).gameObject.SetActive(false);
                Menututorial.SetActive(false);
                saltadoTutorial = true;
                pj.Saltar(true);
            }

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Time.timeScale = 1;
                    Menututorial.transform.GetChild(0).gameObject.SetActive(false);
                    Menututorial.SetActive(false);
                    saltadoTutorial = true;
                    pj.Saltar(true);
                }
            }
        }
        else
        {
            if (contadorTutorial < 0.5)
            {
                contadorTutorial += Time.deltaTime;
            }
            else
            {
                Time.timeScale = 0;
                Menututorial.SetActive(true);
                Menututorial.transform.GetChild(1).gameObject.SetActive(true);
                contadorTutorial = 0;
                tutorial = 3;
            }
        }
    }

    void Tutorial4()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            Menututorial.transform.GetChild(1).gameObject.SetActive(false);
            Menututorial.SetActive(false);
            pj.Saltar(true);
            esTutorial = false;
            jugando = true;
            PlayerPrefs.SetInt("tutorial", 1);
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Time.timeScale = 1;
                Menututorial.transform.GetChild(1).gameObject.SetActive(false);
                Menututorial.SetActive(false);
                pj.Saltar(true);
                esTutorial = false;
                jugando = true;
                PlayerPrefs.SetInt("tutorial", 1);
            }
        }
    }

    public bool EsTutorial()
    {
        return esTutorial;
    }
    //Desactiva la musica o los sonidos
    void Desactivar(int n , int activar)
    {
        if(n==0)
        {
            aS.volume = activar;
        }
        else
        {
            audioMenu.volume = activar;
            audioPersonaje.volume = activar;
        }
    }
    public void DesactivarMusica()
    {
        int n;
        if(desactivarMusica.isOn)
        {
            n = 1;
            imagenMusica.sprite = sonidoActivado;
        }
        else
        {
            n = 0;
            imagenMusica.sprite = sonidoDesactivado;
        }
        Desactivar(0, n);
        Desactivar(1, n);
        PlayerPrefs.SetInt("musica", n);
    }
    public void DesactivarSonido()
    {
        int n;
        if (desactivarSonido.isOn)
        {
            n = 1;
            imagenSonido.sprite = sonidoActivado;
        }
        else
        {
            n = 0;
            imagenSonido.sprite = sonidoDesactivado;
        }
        Desactivar(1, n);
        PlayerPrefs.SetInt("sonido", n);
    }
    void InicioPartida() //Prepara todo para iniciar la partida
    {
        aS.clip = musicaJugar;
        aS.loop = true;
        aS.Play();
        pj.GetAnimator();
        pj.GetMaterial();
        mensajeGameOver.SetActive(false);
        cont = tiempoEntreSpawn;

        esTutorial = tutorial == 0;

        jugando = !esTutorial;
   

    }

    public void gatoDesbloqueado()
    {
        animDesblq.SetTrigger("activar");
    }

    public void ActualizarAltura(float num, float faltura) //Actualiza la altura actual añadiendo la altura del objeto colocado
    {
        if (!jugando) jugando= true;

        alturaActual = faltura;
        altura = (int)Mathf.Ceil(num * multiplicadorAltura);
        textAltura.text = altura.ToString() + " m";

        if((!select.infoGatos[2].EstaDesbloqueado() && alturaMaxima + altura >= select.totalAltura))
        {
            gatoDesbloqueado();
            select.infoGatos[2].setAct(alturaMaxima + altura);
        }
        if (!select.infoGatos[1].EstaDesbloqueado() && alturaActual >= select.maxAltura)
        {
            gatoDesbloqueado();
            select.infoGatos[1].setAct(altura);
        }

        if (alturaActual > alturaDificultat) spawn.PonerDificil();

        if (!jugandoDeVerdad) jugandoDeVerdad = true;

    }

    public void Play() //Boton para darle al play
    {
        if(select.puedeJugar)
        {
            if(alturaMaxima>0)
            {
                record.transform.position = new Vector3(record.transform.position.x, alturaMaxima / multiplicadorAltura, record.transform.position.z);
                record.SetActive(true);
            }
            recordAltura.enabled = false;
            textAltura.enabled = true;
            InicioPartida();
            altura = 0;
            interfazMenu.SetActive(false);
            PlayerPrefs.SetInt("gato", select.num);
        }
        else
        {
            audioMenu.clip = sonidoStart;
            audioMenu.Play();
        }

    }

    public void Replay() //Vuelve a cargar la escena
    {
        PlayerPrefs.SetInt("anuncio", nAnuncio+1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver() //Muestra el mensaje de Game Over y guarda la altura actual
    {
        if(jugando)
        {
           // Social.ReportScore(altura, "CgkIlteg9sYMEAIQAQ", (bool success) => { });
            finalAltura.text = altura.ToString() + " m";
            textAltura.enabled = false;
            aS.clip = musicaDerrota;
            aS.loop = false;
            aS.Play();

            audioMenu.clip = sonidoStart;

            jugando = false;
            mensajeGameOver.SetActive(true);

            PlayerPrefs.SetInt("alturaTotal", alturaTotal + altura);
            PlayerPrefs.SetInt("Muertes",muertes+1);
            PlayerPrefs.SetInt("esq", esquivados);

            if(!select.infoGatos[4].EstaDesbloqueado() && muertes+1 >= select.infoGatos[4].GetMax())
            {
                gatoDesbloqueado();
                select.infoGatos[4].setAct(muertes + 1);
            }
            if (altura > alturaMaxima)
            {
                PlayerPrefs.SetInt("altura", altura);
                alturaMaxima = altura;
            }
            finalrecordAltura.text = alturaMaxima.ToString() + " m";

            if(altura >= 50) Social.ReportProgress("CgkIlteg9sYMEAIQBw", 100.0f, (bool success) => { });
            if(altura >= 100) Social.ReportProgress("CgkIlteg9sYMEAIQCA", 100.0f, (bool success) => { });
            if (altura >= 200) Social.ReportProgress("CgkIlteg9sYMEAIQCQ", 100.0f, (bool success) => { });
            if (altura >= 300) Social.ReportProgress("CgkIlteg9sYMEAIQCg", 100.0f, (bool success) => { });

        }
    }

    public void MenuOpciones(bool abrir)
    {
        interfazOpciones.SetActive(abrir);
        interfazMenu.SetActive(!abrir);
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                continuarCorrecto();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                continuarCorrecto();
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                Replay();
                break;
        }
    }

    void continuarCorrecto()
    {
        if (!continuar) //Si no ha continuado ya
        {
            PlayerPrefs.SetInt("anuncio", 0);
            nAnuncio = 0;

            PlayerPrefs.SetInt("numCont", numContinuar);

            textAltura.enabled = true;
            contadorContinuar = segundosContinuar + 0.25f;
            ant = 4;
            continuar = true;

            //Esconde el mensajed e game Over
            mensajeGameOver.SetActive(false);
            mensajeGameOver.transform.Find("Continue").gameObject.SetActive(false);

            //Spawnea el dron
            float altuDron = alturaActual;
            if (altuDron < -0.4) altuDron = -0.4f;
            GameObject plat = Instantiate(dron, new Vector3(0, altuDron, 0), dron.transform.rotation);

            //Eliminar objetos
            limitador.GetComponent<Collider2D>().enabled = false;
            limitador.GetComponent<Collider2D>().enabled = true;

            //Reniciar personaje
            pj.Reniciar();
            jugador.transform.position = new Vector3(0, plat.transform.position.y + posicionPersonajeInicio, 0);

            //Activa el contador para volver a empezar
            contadorText.SetActive(true);
            contadorText.GetComponent<Text>().text = segundosContinuar.ToString();
            //Mover camara
            float posCam = plat.transform.position.y + posicionCamaraInicio;
            if (posCam < 6.3f)
            {
                posCam = 6.3f;
            }
            camara.transform.position = new Vector3(0, posCam, camara.transform.position.z);

            //Cambia la altura del spawn
            spawn.CambiarAltura(plat, 1);
        }
    }
    //Coloca todo para continuar desde el ultimo punto
    public void Continuar()
    {
        aS.Stop();

        if (Advertisement.IsReady(reward_id))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show(reward_id, options);

        }

    }

    /*public void mostrarRank(int i)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            if (i == 0) PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIlteg9sYMEAIQAQ");
            else Social.ShowAchievementsUI();
        }
        else
        {
            Social.localUser.Authenticate((bool success) => { });
        }
    }*/

}
