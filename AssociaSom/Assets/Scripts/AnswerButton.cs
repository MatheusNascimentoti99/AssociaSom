using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    private GameController gameController;
    private DataObject data;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandleClick()
    {
        gameController.AnswerButtonClicked(data.RigthAnswer, data);
    }
    public void Setup(DataObject objData, bool isCerta)
    {
        data = objData;
        data.RigthAnswer = isCerta;
    }

   
}
