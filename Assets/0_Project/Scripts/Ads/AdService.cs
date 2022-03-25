using System;
using System.Threading.Tasks;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Ads
{
    [CreateAssetMenu(fileName = "AdService", menuName = "Services/AdService")]
    public class AdService : ScriptableObject, INeedInitialization
    {
        private static DateTime LastTimeAdShown;

        private readonly MobirayLogger _logger = new MobirayLogger("AdService");
        
        private GameSettings _settings;
        private IAdProvider _adProvider;
        
        private TaskCompletionSource<RewardedVideoStatus> _taskRewardedVideoCallback;
        
        public void Initialize()
        {
            _settings = ToolBox.Get<GameSettings>();
            // _adProvider = AdAdapterAppLovin.Instance;
        }

        public bool IsInterstitialReady() { return _adProvider.IsInterstitialReady(); }

        public bool ShowInterstitial(AdPlacement placement, bool force = false)
        {
            var time = (DateTime.Now - LastTimeAdShown).TotalSeconds;
            
            if (time < _settings.interstitialCoolDown && !force)
            {
                _logger.LogDebug($"Try to show interstitial failed: time {time}");
                return false;
            }

            if (!_adProvider.IsInterstitialReady())
            {
                _logger.LogDebug($"Try to show interstitial failed : not ready");
                Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.INTERSTITIAL, placement, false));
                return false;
            }
            
            _logger.LogDebug($"Try to show interstitial success");

            Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.INTERSTITIAL, placement, true));
            Analytics.OnEventVideoAdsStarted(EventAds.EventStart(AdType.INTERSTITIAL, placement));
            
            if (_settings.adsFreeBuild)
            {
                _logger.LogDebug($"Free Ads Build! No interstitial");
                return true;
            }
            
            _adProvider.ShowInterstitial(placement, OnInterstitialCallback);

            return true;
        }

        private void OnInterstitialCallback(bool shown)
        {
            LastTimeAdShown = DateTime.Now;
        }

        public bool IsRewardedVideoReady() { return _adProvider.IsRewardedVideoReady(); }

        public void ShowRewardedVideo(AdPlacement placement, Action<RewardedVideoStatus> callback)
        {
            if (_settings.adsFreeBuild)
            {
                _logger.LogDebug($"Free Ads Build! No RV");
                callback.Invoke(RewardedVideoStatus.REWARDED);
                return;
            }
            
            if (!_adProvider.IsRewardedVideoReady())
            {
                _logger.LogDebug("Show rewarded video failed : not loaded");
                Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.VIDEO, placement, false));
                
                callback?.Invoke(RewardedVideoStatus.NOT_LOADED);
                return;
            }
            
            _logger.LogDebug("Show rewarded video success");
                
            Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.VIDEO, placement, true));
            Analytics.OnEventVideoAdsStarted(EventAds.EventStart(AdType.VIDEO, placement));
            
            _adProvider.ShowRewardedVideo(placement, callback);
        }

        public async Task<RewardedVideoStatus> ShowRewardedVideo(AdPlacement placement)
        {
            _taskRewardedVideoCallback = new TaskCompletionSource<RewardedVideoStatus>();
            
            ShowRewardedVideo(placement, OnRewardedVideoCallback);
            
            var status = await _taskRewardedVideoCallback.Task;
            
            _taskRewardedVideoCallback = null;

            if (status == RewardedVideoStatus.REWARDED)
            {
                LastTimeAdShown = DateTime.Now;
            }
            
            return status;
        }
        
        private void OnRewardedVideoCallback(RewardedVideoStatus status)
        {
            _logger.LogDebug($"Rewarded Video Callback : {status}");

            _taskRewardedVideoCallback?.SetResult(status);
        }
    }
}