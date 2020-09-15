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
using System.Linq;

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
    private ConfigController config;

    private List<DataObject> figuras;
    public int quantOpcoes;
    private int erros;
    private int quantRodada;
    List<GameObject> answerButtonGameObjects = new List<GameObject>();
    public Falar call;
    private double higthScore;
    private bool isShow;
    private string localHighestScore;
    private Pontuacao highestScore;
    public AudioSource musicaFundo;

    private int rodadaDica;
    private int quantDica;
    private Firebase.Database.FirebaseDatabase dbInstance;
    private bool running;
    private AudioClip soundRight;
    // Start is called before the first frame update
    async void Start()
    {
        config = FindObjectOfType<ConfigController>();
        musicaFundo.mute = false;
        Debug.Log(!config.getMusica());
        if (!config.getMusica())
            musicaFundo.mute = true;
        else
            musicaFundo.mute = false;
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
            _ErroInternet();
        }
        quantDica = 5;
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

        if (config.getImportFiguras())
        {
            figuras = dataController.Load();
            _AvisoPersonalizado("Carregando figuras e áudios...");
            await dbInstance.GetReference("figuras").GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _ErroInternet();
                    // Handle the error...
                }
                else if (task.IsCompleted)
                {
                    foreach (DataSnapshot figura in task.Result.Children)
                    {

                        IDictionary discFigura = (IDictionary)figura.Value;
                        DataObject data = new DataObject(discFigura["nomeFigura"].ToString(),
                            discFigura["dica"].ToString(), discFigura["localImagem"].ToString(),
                            discFigura["localAudio"].ToString(), (bool)discFigura["rigthAnswer"], false);
                        Debug.Log(data.GetNomeFigura());
                        figuras.Add(data);
                        Debug.Log(figuras.Count);

                    }

                }
            });
        }
        else
        {
            figuras = dataController.Load();
        }

        Debug.Log("Teste");
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
        FindObjectOfType<ConfigController>().LoadConfig();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && dicaController.panelDica.gameObject.activeSelf)
        {
            dicaController.panelDica.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
            SceneManager.LoadScene("Menu");
        }

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
            quantDica++;
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

    public void FalarButton()
    {
        audioSource.clip = soundRight;
        audioSource.Play();
    }


    IEnumerator GetAudioClip()
    {
        string localFile = rodada.figuraCerta.GetLocal() ? "file://" + rodada.figuraCerta.GetLocalAudio() : rodada.figuraCerta.GetLocalAudio();
        using (UnityWebRequest www = config.getImportFiguras() ? UnityWebRequestMultimedia.GetAudioClip(localFile, AudioType.MPEG) : UnityWebRequestMultimedia.GetAudioClip(localFile, AudioType.WAV))
        {
            if (www.uploadProgress < 1)
            {
                _AvisoPersonalizado("Carregando Audio...");
            }
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                _ErroInternet();
                Debug.Log(www.error);
            }
            else
            {
                soundRight = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = soundRight;
                aviso.gameObject.SetActive(false);
            }
        }
    }


    public void AnswerButtonClicked(bool estaCorreto, DataObject data)
    {
        string [] acertos = { "Parabéeens!", "Muito bem!", "Está indo bem!", "Você acertou!", "Ótimo!", "Continue assim!" };
        audioSource.Stop();
        if (data.Equals(rodada.figuraCerta))
        {
            if(quantRodada % 3 == 0)
                ReproduzirNome(acertos[Random.Range(0, acertos.Length)]);
            else
            {
                audioSource.clip = Resources.Load<AudioClip>("right");
                audioSource.Play();
            }
            higthScore += 100 * quantRodada / rodada.tempo;
            score.text = "Pontuação:" + string.Format("{0:00}", higthScore);
            isShow = false;
            panelFiguras.SetActive(false);
            NovaRodada();
            ShowQuestion(rodada);
        }

        else
        {
            audioSource.clip = Resources.Load<AudioClip>("wrong");
            audioSource.Play();
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

        bool callDica = dicaController.CallDica(rodada.figuraCerta.Dica(), quantDica, rodadaDica, quantRodada);
        if (callDica)
        {
            quantDica = 0;
            rodadaDica = quantRodada;
        }

    }


    public void GameOver()
    {
        AnswerButton[] buttons = GameObject.FindObjectsOfType<AnswerButton>();
        foreach (AnswerButton element in buttons)
        {
            Debug.Log("Aqui");
            element.gameObject.SetActive(false);
        }
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
            SaveScore(pontuacao, localHighestScore);
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

        RemoveAnswerButtons();
        for (int i = 0; i < rodada.opcoes.Count; i++)
        {
            StartCoroutine(GetText(rodada.opcoes[i]));
        }
        StartCoroutine(GetAudioClip());
    }

    IEnumerator GetText(DataObject data)
    {
        string localFile = data.GetLocal() ? "file://" + data.GetLocalImagem() : data.GetLocalImagem();
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(localFile))
        {
            Debug.Log(data.GetLocalImagem());
            if (uwr.uploadProgress < 1)
            {
                _AvisoPersonalizado("Carregando imagens...");
            }
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                _ErroInternet();
                Debug.Log(uwr.error);
            }
            else if (answerButtonGameObjects.Count == 4)
            {
                
            }
            else
            {
                // Get downloaded asset bundle
                GameObject answerButtonObject = answerButtonOjbectPool.GetObject();
                answerButtonObject.transform.SetParent(answerButtonParent);

                AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
                answerButton.Setup(data, false);
                Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
                answerButton.GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(1, 1));
                answerButtonGameObjects.Add(answerButtonObject);
                aviso.gameObject.SetActive(false);
                isShow = true;
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
    private void _ErroInternet()
    {
        aviso.gameObject.SetActive(true);
        aviso.text = "Sem conexão com a internet";
    }

    private void _AvisoPersonalizado(string text)
    {
        aviso.gameObject.SetActive(true);
        aviso.text = text;
    }
}

