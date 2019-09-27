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
    /// Represents android.speech.tts.Voice object
    /// </summary>
    [Serializable]
    public class Voice
    {
        /// <summary>
        /// Gets the name of the voice
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the AndroidJavaObject which represents the Voice.
        /// </summary>
        public AndroidJavaObject JavaObject { get; private set; }

        public Voice(AndroidJavaObject obj)
        {
            this.JavaObject = obj;
            this.Name = obj.Call<string>("getName");
        }
    }

}
