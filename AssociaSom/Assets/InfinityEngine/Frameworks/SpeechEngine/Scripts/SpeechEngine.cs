/*************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/

namespace InfinityEngine.Localization
{

    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;
    using DesignPatterns;
    using System.Linq;

    /// <summary>
    ///  Handles android Text to speech api. 
    /// </summary>
    public class SpeechEngine : Singleton<SpeechEngine>
    {

        #region Fields

        private Locale mCurrentLocale;
        private Voice mCurrentVoice;

        private AndroidJavaObject javaLocaleObj;
        private AndroidJavaObject javaVoiceObj;
        private AndroidJavaObject javaEngineObj;

        private AndroidJavaObject engine;
        private AndroidJavaObject context;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The enabled status of the engine
        /// </summary>
        public static bool IsEnabled
        {
            get => PlayerPrefs.GetInt("ENABLED_SPEECH_ENGINE", 1) == 1 && Instance.engine != null;
            set => PlayerPrefs.SetInt("ENABLED_SPEECH_ENGINE", value ? 1 : 0);
        }

        /// <summary>
        /// The current language of the plugin.
        /// </summary>
        public static Locale CurrentLocale => Instance.mCurrentLocale;

        /// <summary>
        /// The current voice of the plugin.
        /// </summary>
        public static Voice CurrentVoice => Instance.mCurrentVoice;

        /// <summary>
        /// Speech engine pitch value.
        /// </summary>
        public static float PitchValue
        {
            get => PlayerPrefs.GetFloat("SPEECH_ENGINE_PITCH_VALUE", 1.0f);
            set
            {
                if (!IsEnabled)
                    return;
                PlayerPrefs.SetFloat("SPEECH_ENGINE_PITCH_VALUE", value);
                Instance.engine.Call("setPitch", value);
            }
        }

        /// <summary>
        /// Speech engine speech rate value.
        /// </summary>
        public static float SpeechRateValue
        {
            get => PlayerPrefs.GetFloat("SPEECH_ENGINE_SPEECH_RATE_VALUE", 1.0f);
            set
            {
                if (!IsEnabled)
                    return;
                PlayerPrefs.SetFloat("SPEECH_ENGINE_SPEECH_RATE_VALUE", value);
                Instance.engine.Call("setSpeechRate", value);
            }
        }

        /// <summary>
        /// Is the engine speaking ?
        /// </summary>
        public static bool IsSpeaking
        {
            get
            {
                if (!IsEnabled)
                    return false;

                return Instance.engine.Call<bool>("isSpeaking");
            }
        }

        /// <summary>
        /// Is the engine ready to speak ?
        /// </summary>
        public static bool IsReady
        {
            get
            {
                if (!IsEnabled)
                    return false;

                return Instance.engine.Call<bool>("isReady");
            }
        }

        /// <summary>
        /// All availables and supported languages.
        /// </summary>
        public static Locale[] AvailableLocales
        {
            get
            {
                if (!IsEnabled)
                    return null;

                return Instance.engine.Call<AndroidJavaObject[]>("getAvailableLocales")
                            .Select(elem =>
                            new Locale(
                                elem.Call<string>("getDisplayName"),
                                elem.Call<string>("getLanguage"),
                                elem.Call<string>("getCountry")
                                ))
                            .Where(locale => IsSupported(locale))
                            .ToArray();
            }
        }

        /// <summary>
        /// All availables and supported voices (for the current language).
        /// </summary>
        public static Voice[] AvaillableVoices
        {
            get
            {
                if (!IsEnabled)
                    return null;

                return Instance.engine.Call<AndroidJavaObject[]>("getAvailableVoices").
                               Select(elem => new Voice(elem))
                               .ToArray();
            }
        }

        /// <summary>
        /// All installed Text to speech engine on the current device.
        /// </summary>
        public static TTSEngine[] AvailableEngines
        {
            get
            {
                if (!IsEnabled)
                    return null;

                return Instance.engine.Call<AndroidJavaObject[]>("getAvailableEngines")
                               .Select(elem => new TTSEngine(elem))
                               .ToArray();
            }
        }

        /// <summary>
        /// Current status of the speech engine
        /// </summary>
        public static SpeechEngineStatus Status
        {
            get
            {
                if (!IsEnabled)
                    return SpeechEngineStatus.Error;

                return (SpeechEngineStatus)Instance.engine.Call<int>("getStatus");
            }
        }

        #endregion Properties

        #region Methods

        #region Unity

        void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (engine == null)
                Init();
#else
            Debug.LogWarningFormat("SpeechEngine works only on Android device !");
#endif
        }

        /// <summary>
        /// Stop speaking on pause.
        /// </summary>
        /// <param name="pause">pause status</param>
        public void OnApplicationPause(bool pause)
        {
            if (pause)
                Stop();
        }

        /// <summary>
        /// Shutdown the engine on application quit.
        /// </summary>
        public void OnApplicationQuit()
        {
            ShutDown();
        }

        #endregion Unity

        /// <summary>
        /// Initializes the engine whith english language.
        /// </summary>
        private void Init()
        {
            if (engine == null)
            {
                using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    context = activity.GetStatic<AndroidJavaObject>("currentActivity");
                }

                using (var plugin = new AndroidJavaClass("fr.infinityinteractive.infinityengine.SpeechEngine"))
                {
                    if (plugin != null)
                    {
                        engine = plugin.CallStatic<AndroidJavaObject>("instance");
                        engine.Call("setContext", context);
                        engine.Call("initialize");

                        Infinity.When(() => IsReady, () =>
                        {
                            javaLocaleObj = engine.Call<AndroidJavaObject>("getLanguage");
                            javaVoiceObj = engine.Call<AndroidJavaObject>("getVoice");

                            Instance.mCurrentLocale = new Locale
                            (
                                javaLocaleObj.Call<string>("getDisplayName"),
                                javaLocaleObj.Call<string>("getLanguage"),
                                javaLocaleObj.Call<string>("getCountry")
                            );

                            Instance.mCurrentVoice = new Voice(Instance.javaVoiceObj);
                        });


                    }
                }
            }
        }

        /// <summary>
        /// Adds a callback action that will be executed when the engine is ready to speak.
        /// </summary>
        /// <remarks>
        ///     This callback is not invoked is you changes the scene (with the function <see cref="Infinity.LoadLevelAfterDelay(string, float)"/>) before the engine be ready <br/>
        ///     because the function stops all callback.
        ///     So if you uses this method you cannot changes a scene with the function of <see cref="Infinity"/> class.
        /// </remarks>
        /// <param name="onReady">The action to do when the engine is ready to speak</param>
        public static void AddCallback(Action onReady)
        {
            Infinity.When(() => IsReady, onReady.Invoke);
        }

        /// <summary>
        /// Changes the language of the plugin
        /// </summary>
        /// <param name="locale">new language</param>
        public static void SetLanguage(Locale locale)
        {
            if (!IsEnabled)
                return;

            Instance.mCurrentLocale = locale;
            Instance.javaLocaleObj = locale.ToJavaLocaleObject();
            Instance.engine.Call("setLanguage", Instance.javaLocaleObj);
        }

        /// <summary>
        /// Changes the voice of the plugin
        /// </summary>
        /// <param name="voice">new voice</param>
        public static void SetVoice(Voice voice)
        {
            if (!IsEnabled)
                return;

            Instance.mCurrentVoice = voice;
            Instance.javaVoiceObj = voice.JavaObject;
            Instance.engine.Call("setVoice", voice.JavaObject);

        }

        /// <summary>
        /// Changes the current TTS Engine of the plugin
        /// </summary>
        /// <param name="engine">new engine</param>
        public static void SetEngine(TTSEngine engine)
        {
            if (!IsEnabled)
                return;

            Instance.engine.Call("setEngine", engine.JavaObject);
        }

        /// <summary>
        /// Add listeners to UI elements.
        /// </summary>
        /// <param name="toggle">The toggle which control the status of the engine</param>
        /// <param name="pitchSlider">The slider which control the pitch of the engine.</param>
        /// <param name="speechRateSlider">The slider which control the rate of the engine.</param>
        public static void AddListeners(Toggle toggle, Slider pitchSlider, Slider speechRateSlider)
        {
            if (!IsEnabled)
                return;

            if (toggle != null)
            {
                toggle.isOn = IsEnabled;
                toggle.onValueChanged.AddListener((bool value) =>
                {
                    IsEnabled = value;
                    toggle.isOn = IsEnabled;

                });
            }

            if (pitchSlider != null)
            {
                pitchSlider.value = (!IsEnabled) ? 0 : PitchValue;
                pitchSlider.onValueChanged.AddListener((float value) =>
                {
                    PitchValue = value;
                });

            }
            if (speechRateSlider != null)
            {
                speechRateSlider.value = (!IsEnabled) ? 0 : SpeechRateValue;
                speechRateSlider.onValueChanged.AddListener((float value) =>
                {
                    SpeechRateValue = value;

                });
            }


        }

        /// <summary>
        /// Creates new <c>java.util.locale</c> object.
        /// </summary>
        /// <param name="locale">The Locale object which will contains the language and the country</param>
        /// <returns>new <c>AndroidJavaObject</c></returns>
        public static AndroidJavaObject CreateLocale(Locale locale)
        {
            return CreateLocale(locale.LanguageCode, locale.Country);
        }

        /// <summary>
        /// Creates new <c>java.util.locale</c> object.
        /// </summary>
        /// <param name="language">The language of the locale example : en</param>
        /// <param name="country">The country example US</param>
        /// <returns>new <c>AndroidJavaObject</c></returns>
        public static AndroidJavaObject CreateLocale(string language, string country)
        {
            if (!IsEnabled)
                return null;

            return Instance.engine.Call<AndroidJavaObject>("createLocale", language, country);
        }

        /// <summary>
        /// Speaks a message
        /// </summary>
        /// <param name="message">message to speak</param>
        public static bool Speak(string message)
        {
            if (!IsEnabled)
                return false;

            return Instance.engine.Call<bool>("speak", message);
        }

        /// <summary>
        /// Speaks a message and makes pause each time there is "{pause}" in the message during  "time" secs
        /// </summary>
        /// <param name="message">the message to speak</param>
        /// <param name="time">pause time in seconds</param>
        public static void SpeakWithPause(string message, int time)
        {
            if (!IsEnabled)
                return;

            Instance.StartCoroutine(Instance._SpeakWithPause(message, time));
        }

        private IEnumerator _SpeakWithPause(string message, int time)
        {
            var stringSeparators = new string[] { "{pause}" };
            var sentences = message.Split(stringSeparators, StringSplitOptions.None);

            foreach (string s in sentences)
            {
                Speak(s);
                yield return new WaitForSeconds(time);
            }

            yield return null;
        }

        /// <summary>
        /// Stops speaking
        /// </summary>
        public static void Stop()
        {
            if (!IsEnabled)
                return;

            Instance.engine.Call("stop");
        }

        /// <summary>
        /// Makes an pause during '<paramref name="time"/>' secs
        /// </summary>
        /// <param name="time">Pause time in seconds</param>
        public static void Pause(int time)
        {
            if (!IsEnabled)
                return;

            Instance.engine.Call("pause", time * 1000);
        }

        /// <summary>
        /// Shutdown the engine
        /// </summary>
        public static void ShutDown()
        {
            if (!IsEnabled)
                return;

            Instance.engine.Call("shutDown");
        }

        /// <summary>
        /// Checks if the given locale is supported by google text to speach api.
        /// </summary>
        /// <param name="locale">The Locale</param>
        /// <returns></returns>
        public static bool IsSupported(Locale locale)
        {
            if (!IsEnabled)
                return false;

            return Instance.engine.Call<bool>("isSupported", locale.LanguageCode, locale.Country);
        }

    }

    #endregion Methods

}