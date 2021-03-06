﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DicaController : MonoBehaviour
{
    public Text dica;
    public Falar call;
    public GameObject panelDica;
    private ConfigController config;
    // Start is called before the first frame update
    void Start()
    {
        config = FindObjectOfType<ConfigController>();
    }
    public bool CallDica(string textDica, int quantDica, int rodadaDica, int rodadaAtual)
    {
        panelDica.SetActive(!panelDica.activeSelf);
        if (panelDica.activeSelf && (quantDica >4 || rodadaDica == rodadaAtual))
        {
            dica.text = textDica;
            if (config.getAudioDescricao())
                call.Speak(textDica);
            return true;
        }
        else if(panelDica.activeSelf)
        {
            if (5 - quantDica == 1)
            {
                dica.text = "Falta " + (5 - quantDica) + " rodada para usar a dica";
                if (config.getAudioDescricao())
                    call.Speak("Falta " + (5 - quantDica) + " rodada para usar a dica");
            }
            else
            {
                dica.text = "Faltam " + (5 - quantDica) + " rodadas para usar a dica";
                if (config.getAudioDescricao())
                    call.Speak("Faltam " + (5 - quantDica) + " rodadas para usar a dica");
            }
            return false;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
