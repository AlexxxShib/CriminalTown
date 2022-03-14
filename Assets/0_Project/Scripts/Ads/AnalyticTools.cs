using System;

namespace CriminalTown.Ads
{
    public static class AnalyticTools
    {
        public static string ToStringValue(this AdType adType)
        {
            switch (adType)
            {
                case AdType.VIDEO:
                    return "rewarded";
                case AdType.INTERSTITIAL:
                    return "interstitial";
                case AdType.BANNER:
                    return "banner";
            }

            return string.Empty;
        }

        public static string ToStringValue(this AdWatchResult watchResult)
        {
            switch (watchResult)
            {
                case AdWatchResult.WATCHED:
                    return "watched";
                case AdWatchResult.CLICKED:
                    return "clicked";
                case AdWatchResult.CANCELED:
                    return "canceled";
            }
            
            return string.Empty;
        }

        public static string ToStringValue(this AdPlacement placement)
        {
            switch (placement)
            {
                case AdPlacement.UNKNOWN:
                    return "unknown";
                
                case AdPlacement.GAME_OVER:
                    return "game_over";
                
                case AdPlacement.LEVEL_COMPLETE:
                    return "level_complete";
                
                case AdPlacement.RV_SKIP_LEVEL:
                    return "rv_skip_level";
            }

            return string.Empty;
        }
    }
}