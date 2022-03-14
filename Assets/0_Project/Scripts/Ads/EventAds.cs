using System.Collections.Generic;
using System.Threading.Tasks;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Ads
{
    public enum AdType
    {
        VIDEO,
        INTERSTITIAL,
        BANNER
    }
    
    public struct EventAds
    {
        public AdType adType;
        public AdPlacement placement;
        public string result;
        
        public bool Connection => Application.internetReachability != NetworkReachability.NotReachable;

        public static EventAds EventAvailable(AdType adType, AdPlacement placement, bool result)
        {
            return new EventAds
            {
                adType = adType, placement = placement, result = result ? "success" : "not_available"
            };
        }
        
        public static EventAds EventStart(AdType adType, AdPlacement placement)
        {
            return new EventAds
            {
                adType = adType, placement = placement, result = "start"
            };
        }

        public static EventAds EventWatch(AdType adType, AdPlacement placement, AdWatchResult watchResult)
        {
            return new EventAds
            {
                adType = adType, placement = placement, result = watchResult.ToStringValue()
            };
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                {
                    "ad_type", adType.ToStringValue()
                },
                {
                    "placement", placement.ToStringValue()
                },
                {
                    "result", result
                },
                {
                    "connection", Connection.ToInt()
                },
            };
        }
    }
}