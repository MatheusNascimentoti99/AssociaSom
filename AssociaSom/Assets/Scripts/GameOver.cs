using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Image imagem;
    public InputField nomeJogador;
    public Text aviso;
    public Text rodada;
    public Text recordeAtual;
    public Button submit;
    public Configuracao config;
    public AudioSource audio;
    public void Recorde(int quantRodada, double record)
    {
        aviso.text = "Parabéns! Você agora tem a melhor pontuação do jogo";
        submit.gameObject.SetActive(true);
        nomeJogador.gameObject.SetActive(true);
        imagem.sprite = Resources.Load<Sprite>("sucess");
        audio.clip = Resources.Load<AudioClip>("win");
        audio.loop = false;
        audio.Play();
        rodada.text = "Você parou na rodada: " + quantRodada;
        recordeAtual.text = "Antigo recorde: " + string.Format("{0:00}", record);
    }

    public void Failed(int quantRodada, double record)
    {
        
        aviso.text = "Não foi dessa vez! :/\n Tente novamente quando achar melhor";
        submit.gameObject.SetActive(false);
        nomeJogador.gameObject.SetActive(false);
        imagem.sprite = Resources.Load<Sprite>("failed");
        rodada.text = "Você parou na rodada: " + quantRodada;
        recordeAtual.text = "Recorde atual: " + string.Format("{0:00}", record);
        audio.clip = Resources.Load<AudioClip>("fail");
        audio.loop = false;
        audio.Play();
    }

}
