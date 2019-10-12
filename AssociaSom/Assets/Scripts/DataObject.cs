﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class DataObject
{
    private string nomeFigura;
    private string dica;
    private string localImagem;
    private string localAudio;
    public bool rigthAnswer;


    public DataObject(string nomeFigura, string dica, string localImagem, string localAudio, bool rigthAnswer)
    {
        this.dica = dica;
        this.nomeFigura = nomeFigura;
        this.localAudio = localAudio;
        this.localImagem = localImagem;
        this.rigthAnswer = rigthAnswer;
    }
    public DataObject()
    {

    }

    public string GetNomeFigura()
    {
        return nomeFigura;
    }
    public string Dica()
    {
        return dica;
    }
    public string GetLocalImagem()
    {
        return localImagem;
    }
    public string GetLocalAudio()
    {
        return localAudio;
    }

    public void SetNomeFigura(string nomeFigura)
    {
        this.nomeFigura = nomeFigura;
    }
    public void SetDica(string dica)
    {
        this.dica = dica;
    }
    public void SetLocalImagem(string localImagem)
    {
        this.localImagem = localImagem;
    }
    public void SetLocalAudio(string localAudio)
    {
        this.localAudio = localAudio;
    }
 

}