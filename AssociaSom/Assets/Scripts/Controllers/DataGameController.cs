using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;

public class DataGameController : MonoBehaviour
{
    string localHighestScore;
    private List<DataObject> figuras;
    private string filePath;
    public static DataGameController dataController;

    void Start()
    {
        
    }
    public List<DataObject> getFiguras()
    {
        return figuras;
    }
    void Awake()
    {
        localHighestScore = Application.persistentDataPath + "/Score.up";
        if (dataController == null)
        {
            dataController = this;
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = Application.persistentDataPath + "/figuras.da";
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);

            figuras = (List<DataObject>)bf.Deserialize(file);
            file.Close();
        }
        if (figuras == null)
        {
            figuras = new List<DataObject>();
        }
    }

    public Pontuacao getRecorde()
    {
        Pontuacao pontuacao = null;
        if (File.Exists(localHighestScore))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(localHighestScore, FileMode.Open);
            pontuacao = (Pontuacao)bf.Deserialize(file);
            file.Close();
        }
        return pontuacao;
    }


}
