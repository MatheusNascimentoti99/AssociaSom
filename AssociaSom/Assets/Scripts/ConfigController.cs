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
        loadConfig();
    }
    private void Awake()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://associasom-2ccf9.firebaseio.com/");

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

        filePath = Application.persistentDataPath + "/config.log";
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool SaveConfig()
    {

        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            //Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, config);
            fs.Close();
            return true;
        }
        catch (SerializationException e)
        {
            Debug.Log(e);
            return false;
        }

    }

    private void loadConfig()
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
    }

    public bool getImportFiguras()
    {
        return config.importFiguras;
    }

    public bool getAudioDescricao()
    {
        return config.audioDescricao;
    }

    public void setImport(bool import)
    {
        config.importFiguras = import;
    }

    public void setAudioDescricao(bool audioDesc)
    {
        config.audioDescricao = audioDesc;
    }

    public void setStateImport()
    {
        config.importFiguras = !config.importFiguras;
    }

    public void setStateAudio()
    {
        config.audioDescricao = !config.audioDescricao;
    }

    public List<DataObject> LoadDataBase()
    {
        List<DataObject> figurasImport = new List<DataObject>();
        Firebase.Database.FirebaseDatabase dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
        dbInstance.GetReference("figuras").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot figura in snapshot.Children)
                {
                    IDictionary discFigura = (IDictionary)figura.Value;
                    Debug.Log("Teste: " + discFigura["dica"] + " - " + discFigura["localImagem"]);
                    figurasImport.Add(new DataObject(discFigura["nomeFigura"].ToString(),
                        discFigura["dica"].ToString(), discFigura["localImagem"].ToString(),
                        discFigura["localAudio"].ToString(), (bool)discFigura["rigthAnswer"]));
                }
                
            }
        });
        return figurasImport;
    }
}

