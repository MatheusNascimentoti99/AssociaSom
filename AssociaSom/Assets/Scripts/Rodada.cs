using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rodada
{
    public List<DataObject> opcoes;
    public DataObject figuraCerta;
    public float tempo;
    public int certa;
    public Rodada(int quantidade, DataObject figuraCerta)
    {
        this.figuraCerta = figuraCerta;
        tempo = 1;
        opcoes = new List<DataObject>();
    }

}
