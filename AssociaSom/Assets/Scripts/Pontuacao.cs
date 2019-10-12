using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pontuacao
{
    private double score;
    private string nomeJogador;

    public Pontuacao(double score, string nomeJogador)
    {
        this.score = score;
        this.nomeJogador = nomeJogador;
    }

    public double getScore()
    {
        return score;
    }
    public string getNameJogador()
    {
        return nomeJogador;
    }

}
