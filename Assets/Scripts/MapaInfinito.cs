using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapaInfinito : MonoBehaviour
{

    public GameObject mapaInfinito;
    public Transform parentFondo;

    GameObject ant = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.transform.tag == "infinito")
        {
            if(ant!=null)
            {
                Destroy(ant);
            }

            Instantiate(mapaInfinito, new Vector3(transform.position.x, transform.position.y + 88.1f, transform.position.z), mapaInfinito.transform.rotation, parentFondo);
            ant = collision.gameObject;
        }
    }
}
