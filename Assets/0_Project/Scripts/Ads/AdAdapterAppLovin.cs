using System;
using CriminalTown.Controllers;
using CriminalTown.Data;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Ads
{
    public class AdAdapterAppLovin : MonoSingleton<AdAdapterAppLovin>, IAdProvider
    {
        public bool initialized;

        [Header("Settings")]
        public string sdkKey = "";

        [Space]
        public string adUnitInterstitial = "INTER";
        public string adUnitRewardedVideo = "REWARDED";

        private SessionData _currentSessionDataOnStart;

        public override void Initialize()
        {
            base.Initialize();

            MaxSdkCallbacks.OnSdkInitializedEvent += OnSdkInitialized;

            MaxSdk.SetHasUserConsent(true);

            MaxSdk.SetSdkKey(sdkKey);
            MaxSdk.InitializeSdk();
        }

        private void OnSdkInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            initialized = true;

            InitializeInterstitialAds();
            InitializeRewardedAds();
        }

        #region INTERSTITIAL

        private int _interstitialRetryAttempt;
        
        private AdPlacement _lastInterstitialPlacement = AdPlacement.UNKNOWN;
        private AdWatchResult _interstitialWatchResult = AdWatchResult.NONE;

        private Action<bool> _interstitialCallback;

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

        private void LoadInterstitial() { MaxSdk.LoadInterstitial(adUnitInterstitial); }
        
        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            _interstitialRetryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Interstitial ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

            _interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _interstitialRetryAttempt));

            Invoke(nameof(LoadInterstitial), (float) retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _interstitialWatchResult = AdWatchResult.WATCHED;
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            
            try
            {
                _interstitialCallback?.Invoke(false);
                _interstitialCallback = null;

            } catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            _interstitialWatchResult = AdWatchResult.CLICKED;
        }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (_interstitialWatchResult != AdWatchResult.NONE)
            {
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.INTERSTITIAL, _lastInterstitialPlacement, _interstitialWatchResult), _currentSessionDataOnStart);

                _interstitialWatchResult = AdWatchResult.NONE;

                try
                {
                    _interstitialCallback?.Invoke(true);
                    _interstitialCallback = null;

                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }

        public bool IsInterstitialReady() { return MaxSdk.IsInterstitialReady(adUnitInterstitial); }

        public bool ShowInterstitial(AdPlacement placement, Action<bool> callback)
        {
            if (!initialized) return false;

            var sessionData = ToolBox.Get<SessionData>();
            _currentSessionDataOnStart = sessionData == null ? null : new SessionData(sessionData);
            
            _lastInterstitialPlacement = placement;
            _interstitialWatchResult = AdWatchResult.NONE;

            _interstitialCallback = callback;

            MaxSdk.ShowInterstitial(adUnitInterstitial);
            return true;
        }

        #endregion

        #region REWARDED_VIDEO
        
        private int _rewardedVideoRetryAttempt;
        private Action<RewardedVideoStatus> _rewardedVideoCallback;
        
        private AdPlacement _lastRewardedVideoPlacement = AdPlacement.UNKNOWN;
        private bool _clickedOnRewardedVideo;

        public bool IsRewardedVideoReady() { return MaxSdk.IsRewardedAdReady(adUnitRewardedVideo); }

        public void ShowRewardedVideo(AdPlacement placement, Action<RewardedVideoStatus> callback)
        {
            if (!initialized)
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
            _currentSessionDataOnStart = sessionData == null ? null : new SessionData(sessionData);

            _clickedOnRewardedVideo = false;

            _lastRewardedVideoPlacement = placement;
            _rewardedVideoCallback = callback;

            MaxSdk.ShowRewardedAd(adUnitRewardedVideo);
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
        
        private void LoadRewardedAd() { MaxSdk.LoadRewardedAd(adUnitRewardedVideo); }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

            // Reset retry attempt
            _rewardedVideoRetryAttempt = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            // Rewarded ad failed to load 
            // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

            _rewardedVideoRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, _rewardedVideoRetryAttempt));

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
            _clickedOnRewardedVideo = true;
        }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();

            if (CallbackRVStatus(RewardedVideoStatus.HIDDEN))
            {
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.VIDEO, _lastRewardedVideoPlacement, AdWatchResult.CANCELED), _currentSessionDataOnStart);
            }
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.

            if (CallbackRVStatus(RewardedVideoStatus.REWARDED))
            {
                var watchResult = _clickedOnRewardedVideo ? AdWatchResult.CLICKED : AdWatchResult.WATCHED;
                
                Analytics.OnEventVideoAdsWatch(EventAds.EventWatch(
                    AdType.VIDEO, _lastRewardedVideoPlacement, watchResult), _currentSessionDataOnStart);
            }
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
        }

        private bool CallbackRVStatus(RewardedVideoStatus status)
        {
            if (_rewardedVideoCallback != null)
            {
                _rewardedVideoCallback.Invoke(status);
                _rewardedVideoCallback = null;

                return true;
            }

            return false;
        }

        #endregion
        
    }
}