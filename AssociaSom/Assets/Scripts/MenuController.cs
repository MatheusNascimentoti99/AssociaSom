using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private DataController data;
    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<DataController>();   
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
