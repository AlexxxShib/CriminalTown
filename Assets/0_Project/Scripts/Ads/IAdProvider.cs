using System;

namespace Template.Ads
{
    public enum AdPlacement
    {
        UNKNOWN,
        GAME_OVER,
        LEVEL_COMPLETE,
        RV_SKIP_LEVEL
    }

    public enum AdWatchResult
    {
        NONE, 
        WATCHED, 
        CLICKED, 
        CANCELED
    }
    
    public enum RewardedVideoStatus
    {
        NOT_LOADED,
        FAILED_TO_DISPLAY,
        HIDDEN,
        REWARDED,
        NOT_INITITLIZED
    }

    public interface IAdProvider
    {
        bool IsInterstitialReady();
        bool ShowInterstitial(AdPlacement placement, Action<bool> callback);

        bool IsRewardedVideoReady();
        void ShowRewardedVideo(AdPlacement placement, Action<RewardedVideoStatus> callback);
    }
}