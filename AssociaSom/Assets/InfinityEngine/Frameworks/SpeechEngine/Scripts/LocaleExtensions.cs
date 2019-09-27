/*************************************************************************************************************************************
* Developed by Mamadou Cisse                                                                                                        *
* Mail => mciissee@gmail.com                                                                                                        *
* Twitter => http://www.twitter.com/IncMce                                                                                          *
* Unity Asset Store catalog: http://u3d.as/riS	                                                                                    *
*************************************************************************************************************************************/

namespace InfinityEngine.Localization
{

    using UnityEngine;

    /// <summary>
    /// <see cref="Locale"/> extensions.
    /// </summary>
    public static class LocaleExtensions
    {
        /// <summary>
        /// Converts this <see cref="Locale"/> object to <c>java.util.Locale</c> object and return it.
        /// </summary>
        /// <param name="self">this</param>
        /// <returns><c>java.util.Locale</c>object</returns>
        public static AndroidJavaObject ToJavaLocaleObject(this Locale self)
        {
            return SpeechEngine.CreateLocale(self);
        }

    }


}