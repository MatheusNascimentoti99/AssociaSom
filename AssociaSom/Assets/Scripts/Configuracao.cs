using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class Configuracao
{
    public bool importFiguras;
    public bool audioDescricao; 
    public Configuracao()
    {
        importFiguras = true;
        audioDescricao = true;
    }
}


