using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public AudioSource musicaFundo;
    public ConfigController config;


    // Start is called before the first frame update
    void Start()
    {
        musicaFundo.mute = false;
        Debug.Log(!config.getMusica());
        if (!config.getMusica())
            musicaFundo.mute = true;
    }
    private void Awake()
    {
        config.Up();
        

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
