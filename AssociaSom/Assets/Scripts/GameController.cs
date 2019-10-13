using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;

public class GameController : MonoBehaviour
{
    public DataGameController dataController;
    public AudioSource audioSource;
    public GameOver gameOver;
    public GameObject gameOverPanel;
    public Text score;
    public Text tempo;
    public Button ouvir;
    public Button dica;
    public List<Image> lifes;
    public GameObject panelFiguras;
    private Rodada rodada;
    public SimpleObjectPool answerButtonOjbectPool;
    public Transform answerButtonParent;
    public Text aviso;
    public DicaController dicaController;
    public ConfigController config;

    private DataObject[] figuraShows;
    private List<DataObject> figuras;
    private List<DataObject> figurasImport;
    public int quantOpcoes;
    private int erros;
    private int quantRodada;
    List<GameObject> answerButtonGameObjects = new List<GameObject>();
    public Falar call;
    private double higthScore;
    private bool isShow;
    private string localHighestScore;
    private Pontuacao highestScore;

    private Firebase.Database.FirebaseDatabase dbInstance;
    private bool running;
    // Start is called before the first frame update
    async void  Start()
    {

        var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://associasom-2ccf9.firebaseio.com/");
                dbInstance = Firebase.Database.FirebaseDatabase.DefaultInstance;
           
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        
        figuras = new List<DataObject>();
        localHighestScore = Application.persistentDataPath + "/Score.up";
        
        highestScore = (Pontuacao)LoadScore(localHighestScore);
        if (highestScore == null)
        {
            highestScore = new Pontuacao(0, "");
            
        }
        isShow = false;
        quantRodada = 0;
        erros = 0;

        if (!config.getImportFiguras())
        {
            dataController.Load();
            figuras = dataController.getFiguras();
        }
        else
        {
            var taskImport = await LoadDataBase(figuras);
            Debug.Log("Esperando2 " + running);
            DataSnapshot import = taskImport;
            figurasImport = new List<DataObject>();
            foreach (DataSnapshot figura in import.Children)
            {

                IDictionary discFigura = (IDictionary)figura.Value;
                DataObject data = new DataObject(discFigura["nomeFigura"].ToString(),
                    discFigura["dica"].ToString(), discFigura["localImagem"].ToString(),
                    discFigura["localAudio"].ToString(), (bool)discFigura["rigthAnswer"]);
                Debug.Log(data.GetNomeFigura());
                figurasImport.Add(data);
                
            }
            figuras = figurasImport;
        }

       
        Debug.Log(figuras.Count);
        if (figuras.Count > 1)
        {
            aviso.gameObject.SetActive(false);
            NovaRodada();
            ShowQuestion(rodada);
            FalarButton();
            tempo.text = rodada.tempo + "s";
        }
        else
        {
            aviso.gameObject.SetActive(true);
            aviso.text = "Sem figuras suficientes para jogar!\n Necessário pelo menos três figuras cadastradas";
        }

    }

    private void Awake()
    {
        config.Up();
        

    }

    // Update is called once per frame
    void Update()
    {
        panelFiguras.SetActive(isShow);
        if (isShow && rodada != null)
        {
            rodada.tempo += Time.deltaTime;

            tempo.text = string.Format("{0:00.#}", rodada.tempo - 1) + "s";
        }
    }



    void NovaRodada()
    {

        if (figuras.Count > 0)
        {

            quantRodada++;
            int posRadomAllFiguras = Random.Range(0, figuras.Count);
            Debug.Log("Quand:" + figuras.Count + "posicão:" + posRadomAllFiguras);
           
            rodada = new Rodada(quantOpcoes, figuras[posRadomAllFiguras]);
            rodada.figuraCerta = figuras[posRadomAllFiguras];
            rodada.figuraCerta.rigthAnswer = true;
            rodada.opcoes.Add(rodada.figuraCerta);
            Debug.Log("Certa: " + rodada.figuraCerta);
            int i = 1;
            posRadomAllFiguras = Random.Range(0, figuras.Count);
            DataObject figura = figuras[posRadomAllFiguras];
            while (i < quantOpcoes)
            {
                if (!rodada.opcoes.Contains(figura))
                {
                    figura.rigthAnswer = false;
                    rodada.opcoes.Add(figura);
                }
                posRadomAllFiguras = Random.Range(0, figuras.Count);
                figura = figuras[posRadomAllFiguras];
                i++;

            }
            Debug.Log(rodada.opcoes.Count);

            tempo.text = rodada.tempo - 1 + "s";

        }

    }
    private void RemoverLife()
    {
        lifes[erros - 1].gameObject.SetActive(false);
    }

    private void ReproduzirNome(string frase)
    {
        call.Speak(frase);
    }
    private void ReproduzirSom()
    {
        audioSource.Play();

    }
    public void FalarButton()
    {
        ReproduzirSom();

    }


    IEnumerator GetAudioClip()
    {
        string localFile = !config.getImportFiguras() ? "file://" + rodada.figuraCerta.GetLocalAudio() : rodada.figuraCerta.GetLocalAudio();
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(localFile, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }


    public void AnswerButtonClicked(bool estaCorreto, DataObject data)
    {
        if (data.Equals(rodada.figuraCerta))
        {
            higthScore += 100 * quantRodada / rodada.tempo;
            score.text = "Pontuação:" + string.Format("{0:00}", higthScore);
            isShow = false;
            panelFiguras.SetActive(false);
            NovaRodada();
            ShowQuestion(rodada);
            FalarButton();
        }

        else
        {
            if(config.getAudioDescricao())
                ReproduzirNome(data.GetNomeFigura()+", Não!");
            erros++;
            if (erros > 3)
            {
                GameOver();
            }
            else
            {
                RemoverLife();
            }
        }



    }

    public void Dica()
    {
        dicaController.CallDica(rodada.figuraCerta.Dica());
    }


    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (highestScore.getScore() < higthScore)
        {
            gameOver.Recorde(quantRodada, highestScore.getScore());
        }
        else
        {
            gameOver.Failed(quantRodada, highestScore.getScore());
        }
    }
    public void SalvarScore()
    {
        if (highestScore.getScore() < higthScore)
        {
            Pontuacao pontuacao = new Pontuacao(higthScore, gameOver.nomeJogador.text);
            SaveScore(pontuacao ,localHighestScore);
            Destroy(gameObject);
            SceneManager.LoadScene("Menu");
        }
        
    }
    private void RemoveAnswerButtons()
    {
        while (answerButtonGameObjects.Count > 0)
        {
            answerButtonOjbectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }

    private void ShowQuestion(Rodada rodada)
    {
        StartCoroutine(GetAudioClip());
        isShow = false;
        RemoveAnswerButtons();
        for (int i = 0; i < rodada.opcoes.Count; i++)
        {
            StartCoroutine(GetText(rodada.opcoes[i]));
            isShow = false;
        }
        isShow = true;


    }

    IEnumerator GetText(DataObject data)
    {
        string localFile = !config.getImportFiguras() ? "file://" + data.GetLocalImagem() : data.GetLocalImagem();
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(localFile))
        {
            Debug.Log(data.GetLocalImagem());
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                GameObject answerButtonObject = answerButtonOjbectPool.GetObject();
                answerButtonObject.transform.SetParent(answerButtonParent);
                answerButtonGameObjects.Add(answerButtonObject);
                AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
                answerButton.Setup(data, false);
                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                answerButton.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
            }
        }
    }

    public bool SaveScore(object obj, string localScore)
    {

        try
        {
            FileStream fs = new FileStream(localScore, FileMode.Create);
            //Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, obj);
            fs.Close();
            return true;
        }
        catch (SerializationException e)
        {
            Debug.Log(e);
            return false;
        }

    }

    public Pontuacao LoadScore(string localScore)
    {
        Pontuacao pontuacao = null;
        if (File.Exists(localScore))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(localScore, FileMode.Open);
            pontuacao = (Pontuacao)bf.Deserialize(file);
            file.Close();
        }
        return pontuacao;
    }

    public async Task<DataSnapshot> LoadDataBase(List<DataObject> figurasImport)
    {
        
        running = true;
        Debug.Log("Esperando1 " + running);
        return await dbInstance.GetReference("figuras").GetValueAsync();
    }



}

