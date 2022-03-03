using System;
using System.Threading.Tasks;
using Mobiray.Common;
using Template.Configs;
using Template.Controllers;
using UnityEngine;

namespace Template.Ads
{
    [CreateAssetMenu(fileName = "AdService", menuName = "Services/AdService")]
    public class AdService : ScriptableObject, INeedInitialization
    {
        private static DateTime lastTimeAdShown;

        private MobirayLogger logger = new MobirayLogger("AdService");
        
        private GameSettings settings;

        private IAdProvider adProvider;
        
        private TaskCompletionSource<RewardedVideoStatus> taskRewardedVideoCallback;
        
        public void Initialize()
        {
            settings = ToolBox.Get<GameSettings>();
            adProvider = AdAdapterAppLovin.Instance;
        }

        public bool IsInterstitialReady() { return adProvider.IsInterstitialReady(); }

        public bool ShowInterstitial(AdPlacement placement, bool force = false)
        {
            var time = (DateTime.Now - lastTimeAdShown).TotalSeconds;
            
            if (time < settings.InterstitialCoolDown && !force)
            {
                logger.LogDebug($"Try to show interstitial failed: time {time}");
                return false;
            }

            if (!adProvider.IsInterstitialReady())
            {
                logger.LogDebug($"Try to show interstitial failed : not ready");
                Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.INTERSTITIAL, placement, false));
                return false;
            }
            
            logger.LogDebug($"Try to show interstitial success");

            Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.INTERSTITIAL, placement, true));
            Analytics.OnEventVideoAdsStarted(EventAds.EventStart(AdType.INTERSTITIAL, placement));
            
            if (settings.AdsFreeBuild)
            {
                logger.LogDebug($"Free Ads Build! No interstitial");
                return true;
            }
            
            adProvider.ShowInterstitial(placement, OnInterstitialCallback);

            return true;
        }

        private void OnInterstitialCallback(bool shown)
        {
            lastTimeAdShown = DateTime.Now;
        }

        public bool IsRewardedVideoReady() { return adProvider.IsRewardedVideoReady(); }

        public void ShowRewardedVideo(AdPlacement placement, Action<RewardedVideoStatus> callback)
        {
            if (settings.AdsFreeBuild)
            {
                logger.LogDebug($"Free Ads Build! No RV");
                callback.Invoke(RewardedVideoStatus.REWARDED);
                return;
            }
            
            if (!adProvider.IsRewardedVideoReady())
            {
                logger.LogDebug("Show rewarded video failed : not loaded");
                Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.VIDEO, placement, false));
                
                callback?.Invoke(RewardedVideoStatus.NOT_LOADED);
                return;
            }
            
            logger.LogDebug("Show rewarded video success");
                
            Analytics.OnEventVideoAdsAvailable(EventAds.EventAvailable(AdType.VIDEO, placement, true));
            Analytics.OnEventVideoAdsStarted(EventAds.EventStart(AdType.VIDEO, placement));
            
            adProvider.ShowRewardedVideo(placement, callback);
        }

        public async Task<RewardedVideoStatus> ShowRewardedVideo(AdPlacement placement)
        {
            taskRewardedVideoCallback = new TaskCompletionSource<RewardedVideoStatus>();
            
            ShowRewardedVideo(placement, OnRewardedVideoCallback);
            
            var status = await taskRewardedVideoCallback.Task;
            
            taskRewardedVideoCallback = null;

            if (status == RewardedVideoStatus.REWARDED)
            {
                lastTimeAdShown = DateTime.Now;
            }
            
            return status;
        }
        
        private void OnRewardedVideoCallback(RewardedVideoStatus status)
        {
            logger.LogDebug($"Rewarded Video Callback : {status}");

            taskRewardedVideoCallback?.SetResult(status);
        }
    }
}