// using Facebook.Unity;
using UnityEngine;

namespace Mobiray.Helpers
{
    public class FacebookStart : MonoBehaviour
    {
        /*void Awake()
        {
#if UNITY_IOS
            if (!FB.IsInitialized)
            {
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                FB.ActivateApp();
            }
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_IOS
            if (!pauseStatus)
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    FB.Init(() => { FB.ActivateApp(); });
                }
            }
#endif
        }

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }*/

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }
}