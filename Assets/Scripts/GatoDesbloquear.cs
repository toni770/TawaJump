using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatoDesbloquear 
{
    int act;
    int max;
    string cond;

    public GatoDesbloquear()
    {
        this.act =0;
        this.max = 0;
        this.cond = "";
    }
    public GatoDesbloquear(int act, int max, string cond)
    {
        this.act = act;
        this.max = max;
        this.cond = cond;
    }

    public string GetCondition()
    {
        return cond;
    }

    public int GetActual()
    {
        return act;
    }

    public int GetMax()
    {
        return max;
    }

    public bool EstaDesbloqueado()
    {
        return act >= max;
    }

    public void setAct(int act)
    {
        this.act = act;
    }

    public float Porcentaje()
    {
        return (float)((act * 100) / max)/100;
    }
}
