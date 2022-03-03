using System;
using Mobiray.Common;
using Template.Controllers;
using Template.Data;
using UnityEngine;

namespace Template.Ads
{
    public class AdAdapterAppLovin : MonoSingleton<AdAdapterAppLovin>, IAdProvider
    {
        public bool Initialized;

        [Header("Settings")]
        public string SdkKey = "";

        [Space]
        public string AdUnitInterstitial = "INTER";
        public string AdUnitRewardedVideo = "REWARDED";

        private SessionData currentSessionDataOnStart;

        public override void Initialize()
        {
            base.Initialize();

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;

            MaxSdk.SetHasUserConsent(true);

            MaxSdk.SetSdkKey(SdkKey);
            MaxSdk.InitializeSdk();
        }

        private void OnSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            Initialized = true;

            InitializeInterstitialAds();
            InitializeRewardedAds();
        }

        #region INTERSTITIAL

        private int interstitialRetryAttempt;
        
        private AdPlacement lastInterstitialPlacement = AdPlacement.UNKNOWN;
        private AdWatchResult interstitalWatchResult = AdWatchResult.NONE;

        private Action<bool> interstitialCallback;

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        private void LoadInterstitial() { MaxSdk.LoadInterstitial(AdUnitInterstitial); }
        
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            interstitialRetryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Invoke(nameof(LoadInterstitial), (float) retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            interstitalWatchResult = AdWatchResult.WATCHED;
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            
            try
            {
                interstitialCallback?.Invoke(false);
                interstitialCallback = null;

            } catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            interstitalWatchResult = AdWatchResult.CLICKED;
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (interstitalWatchResult != AdWatchResult.NONE)
            {
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.INTERSTITIAL, lastInterstitialPlacement, interstitalWatchResult), currentSessionDataOnStart);

                interstitalWatchResult = AdWatchResult.NONE;

                try
                {
                    interstitialCallback?.Invoke(true);
                    interstitialCallback = null;

                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }

        public bool IsInterstitialReady() { return MaxSdk.IsInterstitialReady(AdUnitInterstitial); }

        public bool ShowInterstitial(AdPlacement placement, Action<bool> callback)
        {
            if (!Initialized) return false;

            var sessionData = ToolBox.Get<SessionData>();
            currentSessionDataOnStart = sessionData == null ? null : new SessionData(sessionData);
            
            lastInterstitialPlacement = placement;
            interstitalWatchResult = AdWatchResult.NONE;

            interstitialCallback = callback;

            MaxSdk.ShowInterstitial(AdUnitInterstitial);
            return true;
        }

        #endregion

        #region REWARDED_VIDEO
        
        private int rewardedVideoRetryAttempt;
        private Action<RewardedVideoStatus> rewardedVideoCallback;
        
        private AdPlacement lastRewardedVideoPlacement = AdPlacement.UNKNOWN;
        private bool clickedOnRewardedVideo;

        public bool IsRewardedVideoReady() { return MaxSdk.IsRewardedAdReady(AdUnitRewardedVideo); }

        public void ShowRewardedVideo(AdPlacement placement, Action<RewardedVideoStatus> callback)
        {
            if (!Initialized)
            {
                callback.Invoke(RewardedVideoStatus.NOT_INITITLIZED);
                return;
            }
            
            if (!IsRewardedVideoReady())
            {
                callback.Invoke(RewardedVideoStatus.NOT_LOADED);
                return;
            }
            
            var sessionData = ToolBox.Get<SessionData>();
            currentSessionDataOnStart = sessionData == null ? null : new SessionData(sessionData);

            clickedOnRewardedVideo = false;

            lastRewardedVideoPlacement = placement;
            rewardedVideoCallback = callback;

            MaxSdk.ShowRewardedAd(AdUnitRewardedVideo);
        }
        
        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }
        
        private void LoadRewardedAd() { MaxSdk.LoadRewardedAd(AdUnitRewardedVideo); }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            rewardedVideoRetryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            rewardedVideoRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, rewardedVideoRetryAttempt));

            Invoke("LoadRewardedAd", (float) retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
            
            CallbackRVStatus(RewardedVideoStatus.FAILED_TO_DISPLAY);
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            clickedOnRewardedVideo = true;
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();

            if (CallbackRVStatus(RewardedVideoStatus.HIDDEN))
            {
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.VIDEO, lastRewardedVideoPlacement, AdWatchResult.CANCELED), currentSessionDataOnStart);
            }
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.

            if (CallbackRVStatus(RewardedVideoStatus.REWARDED))
            {
                var watchResult = clickedOnRewardedVideo ? AdWatchResult.CLICKED : AdWatchResult.WATCHED;
                
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.VIDEO, lastRewardedVideoPlacement, watchResult), currentSessionDataOnStart);
            }
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
        }

        private bool CallbackRVStatus(RewardedVideoStatus status)
        {
            if (rewardedVideoCallback != null)
            {
                rewardedVideoCallback.Invoke(status);
                rewardedVideoCallback = null;

                return true;
            }

            return false;
        }

        #endregion
        
    }
}