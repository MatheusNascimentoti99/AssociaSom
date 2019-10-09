/*************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/

namespace InfinityEngine.Localization
{

    /// <summary>
    ///    Speech Engine status
    /// </summary>
    public enum SpeechEngineStatus
    {

        // Generic error
        Error = -1,

        // Ready to speak
        Success = 0,

        // Missing language
        LangNotSupported = 1,

        /// <summary>
        /// Network error
        /// </summary>
        NetworkError = 2
    }
}
