using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;
using System.Runtime.Serialization;
using Firebase.Storage;



public class DataController : MonoBehaviour
{
    public MicrofoneController audio;
    public static DataController dataController;
    private List<DataObject> figuras;
    private string filePath;
    public InputField nomeFigura;
    public InputField dica;
    private string localImagem;
    private string localAudio;
    public Text aviso;
    public Text infoImagem;
    private int teste;

    public List<DataObject> getFiguras()
    {

        return figuras;
    }

    public string GetNomeFigura()
    {
        return nomeFigura.text;
    }
    public string Dica()
    {
        return dica.text;
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
        this.nomeFigura.text = nomeFigura;
    }
    public void SetDica(string dica)
    {
        this.dica.text = dica;
    }
    public void SetLocalImagem(string localImagem)
    {
        this.localImagem = localImagem;
    }
    public void SetLocalAudio(string localAudio)
    {
        this.localAudio = localAudio;
    }
    public void Start()
    {

        if (File.Exists(filePath))
        {
            Load();
        }
        else
        {
            figuras = new List<DataObject>();
        }
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

    }



    void Awake()
    {
        if(dataController == null)
        {
            dataController = this;
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = Application.persistentDataPath + "/figuras.da";
    }
    public void Salvar()
    {
        
        DataObject data = new DataObject();
        data.SetDica(dica.text);
        data.SetLocalImagem(localImagem);
        data.SetNomeFigura(nomeFigura.text);

       // StartCoroutine(UploadImage());

        if ( nomeFigura.text.Length > 0 && dica.text.Length > 0 && audio.audioSource.clip != null && localImagem.Length > 0)
        {
            localAudio = audio.Salvar(nomeFigura.text + "" + figuras.Count);
            data.SetLocalAudio(localAudio);
            figuras.Add(data);
            FileStream fs = new FileStream(filePath, FileMode.Create);
           
            //Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, figuras);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                StartCoroutine(UploadImage());
                fs.Close();
                Destroy(gameObject);
                SceneManager.LoadScene("Menu");
            }

        }
        else
        {
            aviso.text = "Preencha todos os campos";
        }
        teste++;
        Debug.Log(teste);
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);

            figuras =  (List<DataObject>)bf.Deserialize(file);
            file.Close();
        }
        if(figuras == null)
        {
            figuras = new List<DataObject>();
        }
    }

    public void OpenDialogBox(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
               localImagem = path;
               infoImagem.text = path;
            }
        }, "Selecione uma imagem", "image/*", maxSize);
    }

    private IEnumerator UploadImage()
    {
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        Firebase.Storage.StorageReference metadata;
        // Create a storage reference from our storage service
        Firebase.Storage.StorageReference storage_ref = storage.GetReferenceFromUrl("gs://associasom-2ccf9.appspot.com/");
        Firebase.Storage.StorageReference rivers_ref = storage_ref.Child("images/" +nomeFigura.text+".jpeg");
        string fileContents = localImagem;
        //var task = rivers_ref.PutFileAsync("C:\\Users\\Matheus\\Pictures\\GeoGebraImage.png");
        var task = rivers_ref.PutFileAsync("file://"+fileContents);

        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            Debug.Log(task.Exception.ToString());
            nomeFigura.text = "Deu ruim";
        }
        else
        {
            Debug.Log("Finished uploading... Download Url: " + task.Result.ToString() + " " + rivers_ref.ToString());
            nomeFigura.text = localImagem;
            localImagem = rivers_ref.ToString();
            String url = task.Result.ToString();
            Debug.Log(url);
        }
    }




}
