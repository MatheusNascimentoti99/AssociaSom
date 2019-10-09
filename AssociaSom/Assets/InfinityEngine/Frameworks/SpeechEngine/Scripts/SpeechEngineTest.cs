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


public class SpeechEngineTest : MonoBehaviour {

    public CanvasGroup settingPane;

    public Slider pitchSlider;
    public Slider speechRateSlider;

    public InputField input;

    public Dropdown localesDropdown;
    public Dropdown voicesDropdown;
    public Dropdown enginesDropdown;

    private TTSEngine[] engines;
    private Locale[] locales;
    private Voice[] voices;

    public Text informations;

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
       InitSpeechEngine();
#endif
    }

    private void InitSpeechEngine()
    {
        SpeechEngine.AddCallback(() =>
        {
            locales = Locale.AllLocales; // SpeechEngine.AvailableLocales;
            voices = SpeechEngine.AvaillableVoices;
            engines = SpeechEngine.AvailableEngines;

            localesDropdown.AddOptions(locales.Select(elem => elem.Informations).ToList());
            voicesDropdown.AddOptions(voices.Select(elem => elem.Name).ToList());
            enginesDropdown.AddOptions(engines.Select(elem => elem.Label).ToList());

            SpeechEngine.AddListeners(null, pitchSlider, speechRateSlider);

            localesDropdown.onValueChanged.AddListener(value =>
            {
                SpeechEngine.SetLanguage(locales[value]);
                voicesDropdown.ClearOptions();
                voices = SpeechEngine.AvaillableVoices;
                voicesDropdown.AddOptions(voices.Select(elem => elem.Name).ToList());

            });

            voicesDropdown.onValueChanged.AddListener(value =>
            {
                SpeechEngine.SetVoice(voices[value]);
            });

            enginesDropdown.onValueChanged.AddListener(value =>
            {
                SpeechEngine.SetEngine(engines[value]);
            });

        });
    }

    private void Update()
    {
        if(SpeechEngine.IsEnabled && SpeechEngine.IsReady)
        {
            informations.text = "Speech Engine is ready to speak in " + SpeechEngine.CurrentLocale.Name;
        }
        else
        {
            informations.text = string.Format("Speech Engine is not ready to speak\n\nError Type : {0}", SpeechEngine.Status);
        }
    }

    public void OpenSetting()
    {
        settingPane.DOFadeIn().Start();
    }

    public void CloseSetting()
    {
        settingPane.DOFadeOut().Start();
    }

    public void Speak()
    {
        SpeechEngine.Speak(input.text);
    }

}