using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DicaController : MonoBehaviour
{
    public Text dica;
    public Falar call;
    public GameObject panelDica;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void CallDica(string textDica)
    {
        panelDica.SetActive(!panelDica.activeSelf);  
        if(panelDica.activeSelf)
            call.Speak(textDica);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
