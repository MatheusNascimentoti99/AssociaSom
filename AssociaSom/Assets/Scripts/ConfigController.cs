using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ConfigController : MonoBehaviour
{
    private Configuracao config;
    public static ConfigController configController;
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        filePath = Application.persistentDataPath + "/configs.data";
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        if (configController == null)
        {
            configController = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadConfig();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveConfig()
    {

        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            //Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, config);
            fs.Close();
            FindObjectOfType<ConfigController>().LoadConfig();
        }
        catch (SerializationException e)
        {
            Debug.Log(e);
        }

    }

    public void LoadConfig()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            config = (Configuracao)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            config = new Configuracao();
        }
        Debug.Log("Audio:" + config.audioDescricao + ", FIguras: " + config.importFiguras + ", Música: " + config.musica);
    }

    public bool getImportFiguras()
    {
        return config.importFiguras;
    }

    public bool getAudioDescricao()
    {
        return config.audioDescricao;
    }
    public bool getMusica(){
        return config.musica;
    }

    public void setMusica(bool musica){
        config.musica = musica;
    }
    public void setImport(bool import)
    {
        config.importFiguras = import;
    }

    public void setAudioDescricao(bool audioDesc)
    {
        config.audioDescricao = audioDesc;
    }
    public void setStateMusica(){
        config.musica = !config.musica;
    }
    public void setStateImport()
    {
        config.importFiguras = !config.importFiguras;
    }

    public void setStateAudio()
    {
        config.audioDescricao = !config.audioDescricao;
    }


}

