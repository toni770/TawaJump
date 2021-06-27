using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que elimina todos los obstaculos que salglan del area

public class Limitador : MonoBehaviour
{
    public ManagerPartida manager;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.transform.tag != "Player" && collision.transform.tag != "infinito") //Si es un objeto, lo destruye
        {
            if (manager.pj.estaVivo && collision.transform.tag != "suelo")
            {
                manager.esquivados++;
                if(!manager.select.infoGatos[3].EstaDesbloqueado() && manager.esquivados >= manager.select.infoGatos[3].GetMax())
                {
                    manager.gatoDesbloqueado();
                    manager.select.infoGatos[3].setAct(manager.esquivados);
                }
            }
            Destroy(collision.gameObject);
        }
        else //Si es el jugador, lo congela y acaba la partida
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.isKinematic = true;
            manager.GameOver();
        }
    }
}
