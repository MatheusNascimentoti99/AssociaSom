using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class MicrofoneController : MonoBehaviour
{
    public AudioSource audioSource;
    public Button reproduzir;
    public Text txtInformacao;
    private float timer;
    // Start is called before the first frame update
    public void Gravar()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
            _Gravar();
        }
    }
    public void Start()
    {
    }


    private void _Gravar()
    {
        if (!Microphone.IsRecording(null))
        {
            reproduzir.gameObject.SetActive(false);
            audioSource.clip = Microphone.Start(null, false, 5, 44100);
            Debug.Log("teste:" + audioSource.timeSamples);
        }

    }
    internal string Salvar(string nameFile)
    {
        if (audioSource.clip != null)
        {
            return SavWav.Save(nameFile, audioSource.clip);
        }
        return null;
    }

    public void playAudio() {
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Microphone.IsRecording(null))
        {
            timer += Time.deltaTime;
                txtInformacao.text = "Gravando... " + string.Format("{0:00.#}", 5-timer);

        }
        else if (audioSource.clip !=null && !Microphone.IsRecording(null))
        {
            reproduzir.gameObject.SetActive(true);
            txtInformacao.text = "Gravado!";
            timer = 0;
        }

    }

    
}
