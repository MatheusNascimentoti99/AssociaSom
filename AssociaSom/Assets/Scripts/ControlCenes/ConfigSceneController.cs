using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigSceneController : MonoBehaviour
{
    public Toggle enableImport;
    public Toggle enableAudio;
    public Toggle enableMusic;

    public ConfigController config;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void Awake()
    {
        FindObjectOfType<ConfigController>().LoadConfig();
        Debug.Log(config);
        enableAudio.isOn = config.getAudioDescricao();
        enableImport.isOn = config.getImportFiguras();
        enableMusic.isOn = config.getMusica();
        if (config.getImportFiguras())
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");

        if (config.getAudioDescricao())
            enableAudio.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableAudio.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");

        if (config.getMusica())
            enableMusic.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableMusic.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");

    }


    public void ChangeAudio()
    {
        config.setAudioDescricao(enableAudio.isOn);
        if (enableAudio.isOn)
            enableAudio.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableAudio.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");
        config.SaveConfig();
        Debug.Log("Situação:" + enableAudio.isOn);
        Debug.Log("Situação: AUdio- " + config.getAudioDescricao() + "  FIg: " + config.getImportFiguras());

    }

    public void ChangeImport()
    {
        config.setImport(enableImport.isOn);
        if (enableImport.isOn)
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");
        config.SaveConfig();
        Debug.Log("Situação:" + enableImport.isOn);

        Debug.Log("Situação: AUdio- " + config.getAudioDescricao() + "  FIg: " + config.getImportFiguras());
    }


    public void ChangeMusic()
    {
        config.setMusica(enableMusic.isOn);
        if (enableMusic.isOn)
            enableMusic.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableMusic.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");
        config.SaveConfig();
        FindObjectOfType<MenuController>().musicaFundo.mute = !config.getMusica();
        Debug.Log("Situação:" + enableMusic.isOn);

        Debug.Log("Situação: AUdio- " + config.getAudioDescricao() + "  FIg: " + config.getImportFiguras() + ", Musica: " + config.getMusica());
    

    }
    public bool ImportFiguras()
    {
        return false;
    }


}
