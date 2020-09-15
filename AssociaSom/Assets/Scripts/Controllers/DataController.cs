using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;



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
    }
    public void Salvar()
    {

        DataObject data = new DataObject();
        data.SetDica(dica.text);
        data.SetLocalImagem(localImagem);
        data.SetNomeFigura(nomeFigura.text);
        data.SetLocal(true);
        // StartCoroutine(UploadImage());

        if (nomeFigura.text.Length > 0 && dica.text.Length > 0 && audio.audioSource.clip != null && localImagem.Length > 0)
        {
            localAudio = audio.Salvar(nomeFigura.text + "" + figuras.Count);
            data.SetLocalAudio(localAudio);
            figuras.Add(data);
            
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                //Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, figuras);
                fs.Close();
                Destroy(gameObject);
                SceneManager.LoadScene("Menu");
            }
            catch (SerializationException)
            {
                aviso.text = "Erro ao processar informações";
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

            figuras = (List<DataObject>)bf.Deserialize(file);
            file.Close();
        }
        if (figuras == null)
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
            //localImagem = "C:\\Users\\mathe\\Downloads\\AC3_THEME\\uPlay_PC_Wallpaper3_1920x1200.jpg";
        }, "Selecione uma imagem", "image/*", maxSize);
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            SceneManager.LoadScene("Menu");
        }
    }

}