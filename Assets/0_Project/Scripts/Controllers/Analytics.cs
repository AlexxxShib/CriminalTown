using System;
using System.Collections.Generic;
using System.Text;
using Facebook.Unity;
using FlurrySDK;
using Mobiray.Common;
using Template.Ads;
using Template.Data;
using UnityEngine;

namespace Template.Controllers
{
    public class Analytics
    {
        
        enum VideoAdsStatus
        {
            WAITING, STARTDED
        }

        public static bool LogEvents = true;
        
        private static MobirayLogger Logger = new MobirayLogger("Analytics");

        private const string CASH_KEY_STARTED_LEVEL = "cash_started_level";
        
        private static VideoAdsStatus adsStatus = VideoAdsStatus.WAITING;

        public static void OnEventVideoAdsAvailable(EventAds adEvent)
        {
            OnEvent("video_ads_available", adEvent.ToDictionary());
        }
        
        public static void OnEventVideoAdsStarted(EventAds adEvent)
        {
            OnEvent("video_ads_started", adEvent.ToDictionary());

            adsStatus = VideoAdsStatus.STARTDED;
        }
        
        public static void OnEventVideoAdsWatch(EventAds adEvent, SessionData sessionData)
        {
            if (adsStatus != VideoAdsStatus.STARTDED) return;
            
            var parameters = adEvent.ToDictionary();

            if (sessionData != null)
            {
                parameters.AddAllKVPFrom(MakeLevelStartParameters(sessionData));
            }
            
            OnEvent("video_ads_watch", parameters);

            adsStatus = VideoAdsStatus.WAITING;
        }

        private static Dictionary<string, object> MakeLevelStartParameters(SessionData sessionData)
        {
            var levelNum = sessionData.CurrentLevel + 1;

            return new Dictionary<string, object>
            {
                {
                    "level_number", levelNum
                },
                {
                    "level_name", $"level_{levelNum:00}"
                },
                {
                    "level_count", sessionData.LevelCount
                },
                {
                    "level_loop", sessionData.LevelLoop
                },
            };
        }

        public static void SendLevelStart(SessionData sessionData)
        {
            PlayerPrefs.SetInt(CASH_KEY_STARTED_LEVEL, sessionData.CurrentLevel);
            PlayerPrefs.Save();

            OnEvent("level_start", MakeLevelStartParameters(sessionData));
            
            AppMetrica.Instance.SendEventsBuffer();
        }

        public static void SendLevelFinish(SessionData sessionData, bool result)
        {
            var parameters = MakeLevelStartParameters(sessionData);
            
            parameters.Add("result", result ? "win" : "lose");
            parameters.Add("time", sessionData.LevelTime);
            
            OnEvent("level_finish", parameters);
            
            AppMetrica.Instance.SendEventsBuffer();
        }
        
        public static void OnEvent(string eventName)
        {
            try
            {
                AppMetrica.Instance.ReportEvent(eventName);
                
                FB.LogAppEvent(eventName);
                // FirebaseAnalytics.LogEvent(eventName);
                // Flurry.LogEvent(eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void OnEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (LogEvents)
            {
                var builder = new StringBuilder($"C# sendEvent : {eventName}");
                builder.Append(Environment.NewLine);
                
                foreach (var pair in parameters)
                {
                    builder.Append("  ").Append(pair.Key).Append(" : ").Append(pair.Value).Append(Environment.NewLine);
                }
                Logger.LogDebug(builder.ToString());
            }
            
            try
            {
                AppMetrica.Instance.ReportEvent(eventName, parameters);
                
                // FB.LogAppEvent(eventName, parameters: parameters);
                Flurry.LogEvent(eventName, ToStringMap(parameters));
                // FirebaseAnalytics.LogEvent(eventName, ToFirebaseParameters(parameters));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static Dictionary<string, string> ToStringMap(Dictionary<string, object> parameters)
        {
            var result = new Dictionary<string, string>();
            foreach (var parameter in parameters)
            {
                result.Add(parameter.Key, parameter.Value.ToString());
            }

            return result;
        }

        /*private static Parameter[] ToFirebaseParameters(Dictionary<string, object> parameters)
        {
            var result = new List<Parameter>();
            foreach (var parameter in parameters)
            {
                if (parameter.Value is float || parameter.Value is double)
                {
                    result.Add(new Parameter(parameter.Key, (double) parameter.Value));
                    continue;
                }

                if (parameter.Value is int || parameter.Value is long)
                {
                    result.Add(new Parameter(parameter.Key, (long) parameter.Value));
                    continue;
                }

                result.Add(new Parameter(parameter.Key, parameter.Value.ToString()));
            }

            return result.ToArray();
        }*/
    }
}