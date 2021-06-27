using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecoracionMapa : MonoBehaviour
{
    public Sprite[] elementos;
    public SpriteRenderer[] pos;
    int num;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<pos.Length;i++)
        {
            num = Random.Range(0, elementos.Length - 1);
            pos[i].sprite = elementos[num];
        }
        
    }

}
