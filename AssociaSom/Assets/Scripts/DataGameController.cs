using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Firebase.Storage;


public class DataGameController : MonoBehaviour
{
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


    public Texture2D LoadTex(string localImagem)
    {
        Firebase.Storage.StorageReference storageReference =
        Firebase.Storage.FirebaseStorage.DefaultInstance.GetReferenceFromUrl("storage_url");

        var task = storageReference.Child("resource_name").GetBytesAsync(1024 * 1024);
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                }
                else
                {
                    byte[] fileContents = task.Result;
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(fileContents);
                    return texture;
                }
        return null;
    }

}
