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
        Object.Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void OpenCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }
    public void OpenInserirFiguras()
    {
        SceneManager.LoadScene("InserirFiguras");
    }
    public void OpenMelhorPontuacao()
    {
        SceneManager.LoadScene("MelhorPontuacao");
    }

}
