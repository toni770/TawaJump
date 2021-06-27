using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que spawnea los objetos

public class Spawner : MonoBehaviour
{
    public GameObject[] item;

    public float alturaActual = 0;
    public GameObject plataforma;
    public float posXIzq=5, posXDer=-5;
    public float rangoEntreObjetos = 0.5f;
    public bool tutorial = false;
    bool dificil = false;

    int num, pos;

    // Start is called before the first frame update
    void Start()
    {
        CambiarAltura(plataforma,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Spawn(3);
        }
    }

    public void PonerDificil()
    {
        dificil = true;
    }
   //Cambia la altura del spawn por encimad el ultimo objeto colocado
    public void CambiarAltura(GameObject go, float añadido)
    {
        alturaActual = go.transform.position.y + go.GetComponent<Collider2D>().bounds.size.y / 2 + añadido;
    }

    public void Spawn(float velocidad)
    {
        if (dificil) num = Random.Range(0, item.Length);
        else if (tutorial) { num = 7; tutorial = false; }
        else num = Random.Range(0, item.Length / 2);


        //Posiciona el spawn
        pos = Posicionar((item[num].transform.GetChild(0).GetChild(0).GetComponent<Renderer>().bounds.size.y / 2));

        //Spawnea el objeto
        GameObject obj = Instantiate(item[num], transform.position, item[num].transform.rotation);
        //Cambia a donde mira el objeto
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * pos, obj.transform.localScale.y, obj.transform.localScale.z);

        //Cambia hacia donde se mueve el objeto
        obj.GetComponent<Obstaculos>().direccion = new Vector2(Mathf.Clamp(transform.position.x-0,-2,2)*-1,0);

        //Cambia la velocidad del objeto
        obj.GetComponent<Obstaculos>().velocidad = velocidad;

    }

    int Posicionar(float alturaObjeto)
    {
        if (Random.value > 0.5f)
        {
            transform.position = new Vector2(posXIzq, alturaActual+alturaObjeto+rangoEntreObjetos);
            return 1;
        }
        else
        {
            transform.position = new Vector2(posXDer, alturaActual+alturaObjeto+rangoEntreObjetos);
            return -1;
        }

    }
}
