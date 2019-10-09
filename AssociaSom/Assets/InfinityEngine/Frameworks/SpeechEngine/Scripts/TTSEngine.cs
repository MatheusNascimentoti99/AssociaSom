/*************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/
using UnityEngine;
using System;

namespace InfinityEngine.Localization
{
    /// <summary>
    ///     Represents android.speech.tts.TextToSpeech.EngineInfo object
    /// </summary>
    [Serializable]
    public class TTSEngine
    {
        public string Name { get; private set; }

        public string Label { get; private set; }
    
        public AndroidJavaObject JavaObject { get; private set; }

        public TTSEngine(AndroidJavaObject obj)
        {
            this.Name = obj.Get<string>("name");
            this.Label = obj.Get<string>("label");
        }
    }
}
