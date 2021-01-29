using System;
using System.Collections.Generic;
using Facebook.Unity;
// using Firebase.Analytics;
using FlurrySDK;

namespace Template.Controllers
{
    public class Analytics
    {
        public static void OnEvent(string eventName)
        {
            try
            {
                FB.LogAppEvent(eventName);
                // FirebaseAnalytics.LogEvent(eventName);
                Flurry.LogEvent(eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void OnEvent(string eventName, Dictionary<string, object> parameters)
        {
            try
            {
                FB.LogAppEvent(eventName, parameters: parameters);
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