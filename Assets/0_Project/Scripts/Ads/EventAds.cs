using System.Collections.Generic;
using System.Threading.Tasks;
using Mobiray.Common;
using UnityEngine;

namespace Template.Ads
{
    public enum AdType
    {
        VIDEO,
        INTERSTITIAL,
        BANNER
    }
    
    public struct EventAds
    {
        public AdType AdType;
        public AdPlacement Placement;
        public string Result;
        public bool Connection => Application.internetReachability != NetworkReachability.NotReachable;

        public static EventAds EventAvailable(AdType adType, AdPlacement placement, bool result)
        {
            return new EventAds
            {
                AdType = adType, Placement = placement, Result = result ? "success" : "not_available"
            };
        }
        
        public static EventAds EventStart(AdType adType, AdPlacement placement)
        {
            return new EventAds
            {
                AdType = adType, Placement = placement, Result = "start"
            };
        }

        public static EventAds EventWatch(AdType adType, AdPlacement placement, AdWatchResult watchResult)
        {
            return new EventAds
            {
                AdType = adType, Placement = placement, Result = watchResult.ToStringValue()
            };
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                {
                    "ad_type", AdType.ToStringValue()
                },
                {
                    "placement", Placement.ToStringValue()
                },
                {
                    "result", Result
                },
                {
                    "connection", Connection.ToInt()
                },
            };
        }
    }
}