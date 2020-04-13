using FlurrySDK;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class FlurryStart : MonoBehaviour
    {
        public string KEY_ANDROID;
        public string KEY_IOS;

        void Start()
        {
#if UNITY_ANDROID
            string flurryApiKey = KEY_ANDROID;
#elif UNITY_IPHONE
            string flurryApiKey = KEY_IOS;
#else
            string flurryApiKey = null;
#endif

            new Flurry.Builder()
                .WithCrashReporting(true)
                .WithLogEnabled(true)
                .WithLogLevel(Flurry.LogLevel.VERBOSE)
//                .withMessaging(true);
                .Build(flurryApiKey);
        }
    }
}