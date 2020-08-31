using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public AudioSource musicaFundo;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(!FindObjectOfType<ConfigController>().getMusica());
        if (!FindObjectOfType<ConfigController>().getMusica())
             musicaFundo.mute = true;
        else
        {
            musicaFundo.loop = true;
            musicaFundo.mute = false;
        }     
    }
    private void Update()
    {
    }
    private void Awake()
    {
    }

    public void Menu()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("Game");
    }
    public void OpenCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void OpenConfig()
    {
        SceneManager.LoadScene("Configuracoes");
    }

    public void OpenInserirFiguras()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("InserirFiguras");
    }
    public void OpenMelhorPontuacao()
    {
        SceneManager.LoadScene("MelhorPontuacao");
    }

}
