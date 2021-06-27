using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

//Script para seleccionar la skin del personaje
public class SelectorPersonaje : MonoBehaviour
{
    public GameObject[] Gatos;
    public GameObject gatoBloqueado;
    public Transform player;
    public Sprite playBloquedo, playJugando;
    public Image botonPlay;
    public GameObject slider;

    public AudioSource AudioMenu;
    public AudioClip sonidoflecha;
    public int num=0;
    public bool puedeJugar;
    public int maxAltura, totalAltura, esquivar, muerte;

    Slider barraProgreso;
    Text textoBarra;

    GameObject obj;

    [HideInInspector]
    public GatoDesbloquear[] infoGatos;
    private void Start()
    {
        barraProgreso = slider.GetComponent<Slider>();
        textoBarra = slider.GetComponentInChildren<Text>();

        infoGatos = new GatoDesbloquear[Gatos.Length];

        infoGatos[0] = new GatoDesbloquear();
        infoGatos[1] = new GatoDesbloquear(PlayerPrefs.GetInt("altura", 0), maxAltura, "Max height");
        infoGatos[2] = new GatoDesbloquear(PlayerPrefs.GetInt("alturaTotal", 0), totalAltura, "Total height");
        infoGatos[3] = new GatoDesbloquear(PlayerPrefs.GetInt("esq", 0), esquivar, "Dodge");
        infoGatos[4] = new GatoDesbloquear(PlayerPrefs.GetInt("Muertes", 0), muerte, "????");

        /*if(infoGatos[1].EstaDesbloqueado()) Social.ReportProgress("CgkIlteg9sYMEAIQAg", 100.0f, (bool success) => { });
        if(infoGatos[2].EstaDesbloqueado()) Social.ReportProgress("CgkIlteg9sYMEAIQBA", 100.0f, (bool success) => { });
        if(infoGatos[3].EstaDesbloqueado()) Social.ReportProgress("CgkIlteg9sYMEAIQAw", 100.0f, (bool success) => { });
        if(infoGatos[4].EstaDesbloqueado()) Social.ReportProgress("CgkIlteg9sYMEAIQBQ", 100.0f, (bool success) => { });*/

        puedeJugar = true;
        num = PlayerPrefs.GetInt("gato", 0);
        if (num != 0) CambiarPersonaje();

    }
    public void Right()
    {
        if (num == Gatos.Length-1)
        {
            num = 0;
        }
        else
        {
            num++;
        }
        AudioMenu.clip = sonidoflecha;
        AudioMenu.Play();
        CambiarPersonaje();
    }
    public void Left()
    {
        if (num == 0)
        {
            num = Gatos.Length-1;
        }
        else
        {
            num--;
        }
        AudioMenu.clip = sonidoflecha;
        AudioMenu.Play();
        CambiarPersonaje();
    }

    //Cambia de personaje
    void CambiarPersonaje()
    {
        Destroy(player.GetChild(0).gameObject);

        
        if (infoGatos[num].EstaDesbloqueado())
        {
            botonPlay.sprite = playJugando;
            obj = Instantiate(Gatos[num], player);
            puedeJugar = true;
            slider.SetActive(false);
        }
        else
        {
            botonPlay.sprite = playBloquedo;
            obj = Instantiate(gatoBloqueado, player);
            puedeJugar = false;
            slider.SetActive(true);
            textoBarra.text = infoGatos[num].GetCondition() + " " + infoGatos[num].GetActual() + "/" + infoGatos[num].GetMax();
            barraProgreso.value = infoGatos[num].Porcentaje();
        }
        
        obj.transform.localPosition = Vector3.zero;
    }
}
