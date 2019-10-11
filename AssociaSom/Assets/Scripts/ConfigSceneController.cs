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
    public ConfigController config;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ChangeAudio(bool value)
    {
        config.setImport(enableAudio.isOn);
        if(enableAudio.isOn)
            enableAudio.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableAudio.GetComponentInChildren<Image>().sprite= Resources.Load<Sprite>("off");
        config.SaveConfig();
        Debug.Log("Situação:" + enableAudio.isOn);
    }

    public void ChangeImport(bool value)
    {
        config.setAudioDescricao(enableImport.isOn);
        if (enableImport.isOn)
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("on");
        else
            enableImport.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("off");
        config.SaveConfig();
        Debug.Log("Situação:" + enableImport.isOn);
    }

    

    public bool ImportFiguras()
    {
        return false;
    }


}
