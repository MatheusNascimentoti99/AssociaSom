///#EXIT
/************************************************************************************************************************************													   *
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using InfinityEngine.Localization;
using InfinityEngine.Extensions;
using System.Linq;


public class Falar : MonoBehaviour {


    private TTSEngine[] engines;
    private Locale[] locales;
    private Voice[] voices;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       InitSpeechEngine();
#endif
    }

    private void InitSpeechEngine()
    {
  
    }

    private void Update()
    {

    }
    public bool IsSpeak()
    {
        return SpeechEngine.IsSpeaking;
    }

    public void Speak(string input)
    {
        SpeechEngine.Speak(input);
        
    }

}