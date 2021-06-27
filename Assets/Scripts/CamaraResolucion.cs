using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script que cambia el tamaño de la camara dependiendo de la resolucion del movil.
public class CamaraResolucion : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Camera>().projectionMatrix = Matrix4x4.Ortho(-11.48f * 0.5f, 11.48f * 0.5f, -11.48f, 11.48f, 0.3f, 1000f);
    }
}
