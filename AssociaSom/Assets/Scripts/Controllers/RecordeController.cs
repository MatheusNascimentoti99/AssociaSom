using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordeController : MonoBehaviour
{
    public DataGameController gameController;
    public Text nomeRecorde;
    // Start is called before the first frame update
    void Start()
    {
        nomeRecorde.text = gameController.getRecorde().getNameJogador() + ": " + string.Format("{0:00}", gameController.getRecorde().getScore()) + " Pontos";   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
