using System;
using System.Collections.Generic;
using System.Text;
using Facebook.Unity;
// using Firebase.Analytics;
using FlurrySDK;
using Template.Configs;
using Template.Data;
using Mobiray.Common;
using UnityEngine;

namespace Template.Controllers
{
    public class Analytics
    {

        public static bool LogEvents = true;
        
        private static MobirayLogger Logger = new MobirayLogger("Analytics");

        /*public static void SendLevelStart(DataGameState gameState, ConfigMain configMain)
        {
            // var levelNum = gameState.CurrentLevel + 1;
            configMain.GetLevelConfig(gameState.CurrentLevel, out var levelNum, out var loop);
            var startedLevel = PlayerPrefs.GetInt("cash_started_level", -1);
            
            if (startedLevel == levelNum) return;
            
            PlayerPrefs.SetInt("cash_started_level", levelNum);
            PlayerPrefs.Save();

            OnEvent("level_start", new Dictionary<string, object>
            {
                {
                    "level_number", levelNum
                },
                {
                    "level_name", $"level_{levelNum}"
                },
                {
                    "level_count", gameState.CurrentLevel
                },
                {
                    "level_loop", loop
                },
            });
        }*/

        /*public static void SendLevelFinish(DataGameState gameState, ConfigMain configMain, bool result)
        {
            configMain.GetLevelConfig(gameState.CurrentLevel, out var levelNum, out var loop);
            
            OnEvent("level_finish", new Dictionary<string, object>
            {
                {
                    "level_number", levelNum
                },
                {
                    "level_name", $"level_{levelNum}"
                },
                {
                    "level_count", gameState.CurrentLevel
                },
                {
                    "level_loop", loop
                },
                {
                    "result", result ? "win" : "lose"
                }
            });
        }*/
        
        public static void OnEvent(string eventName)
        {
            try
            {
                FB.LogAppEvent(eventName);
                // FirebaseAnalytics.LogEvent(eventName);
                Flurry.LogEvent(eventName);
                
                // AppMetrica.Instance.ReportEvent(eventName);
                // AppMetrica.Instance.SendEventsBuffer();
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
                FB.LogAppEvent(eventName, parameters: parameters);
                Flurry.LogEvent(eventName, ToStringMap(parameters));
                // FirebaseAnalytics.LogEvent(eventName, ToFirebaseParameters(parameters));
                
                // AppMetrica.Instance.ReportEvent(eventName, parameters);
                // AppMetrica.Instance.SendEventsBuffer();
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