using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Mobiray.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace Mobiray.Common
{
    public static class Tools
    {
        public static void NextFrame(this MonoBehaviour mono, Action action)
        {
            mono.StartCoroutine(OneFrame(action));
        }

        private static IEnumerator OneFrame(Action action)
        {
            yield return null;

            action.Invoke();
        }

        public static void StartTimer(this MonoBehaviour mono, float time, Action action)
        {
            mono.StartCoroutine(StartTimer(time, action));
        }

        private static IEnumerator StartTimer(float time, Action action)
        {
            yield return new WaitForSeconds(time);

            action.Invoke();
        }

        public static Vector3 ChangeX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        public static Vector3 ChangeY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 ChangeZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        public static Vector3 ToVector(this float value)
        {
            return new Vector3(value, value, value);
        }

        public static Vector3 GetRandomPointInArea(Vector3 center, float radius)
        {
            float randomAngle = Random.Range(0f, 2 * Mathf.PI);
            float randomRadius = Random.Range(0f, radius);

            return center + new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * randomRadius;
        }

        public static Vector3 GetRandomPointInArea(this GizmoSphere gizmo)
        {
            return GetRandomPointInArea(gizmo.transform.position, gizmo.Radius);
        }

        public static bool Chance(this float chance)
        {
            chance = Mathf.Clamp01(chance);
            return Random.Range(0f, 1f) < chance;
        }

        public static bool RandomBool()
        {
            return Random.Range(0f, 1f) < .5f;
        }

        public static T RandomItem<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

//        public delegate bool Logic<T>(T element);
//
//        public static T First<T>(this IEnumerable<T> list, Logic<T> fun) where T : class {
//            return list.FirstOrDefault(element => fun(element));
//        } 

        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(0, list.Count);

                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        public static T RandomItemWeights<T>(this List<T> list, float[] weights)
        {
            if (list.Count != weights.Length)
            {
                throw new ArgumentException("Illegal size for weights array");
            }

            var sum = weights.Sum();
            var random = Random.Range(0, sum);

            var counter = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                if (counter < random && random < counter + weights[i])
                {
                    return list[i];
                }

                counter += weights[i];
            }

            return list[list.Count - 1];
        }

        public static T RandomItemEdges<T>(this List<T> list, float[] chanceEdges)
        {
            if (list.Count - 1 != chanceEdges.Length)
            {
                throw new ArgumentException("Illegal size for chances array");
            }

            var random = Random.Range(0, 1);
            var counter = 0f;

            bool found = false;
            int index;
            for (index = 0; index < chanceEdges.Length; index++)
            {
                if (counter < random && chanceEdges[index] >= random)
                {
                    found = true;
                    break;
                }

                counter = chanceEdges[index];
            }

            if (found)
            {
                return list[index];
            }

            return list[list.Count - 1];
        }

        public static void Reverse(this StringBuilder sb)
        {
            for (int i = 0, j = sb.Length - 1; i < sb.Length / 2; i++, j--)
            {
                char chT = sb[i];

                sb[i] = sb[j];
                sb[j] = chT;
            }
        }

        public static float Progress(float start, float end, float cur)
        {
            end -= start;
            cur -= start;

            return cur / end;
        }

        public static BigInteger Multiply(BigInteger leftSide, float factor, int shape = 1000)
        {
            var rightSide = (BigInteger) (factor * shape);
            return leftSide * rightSide / shape;
        }

        public static BigInteger Multiply(BigInteger leftSide, double factor, int shape = 10000000)
        {
            var rightSide = (BigInteger) (factor * shape);
            return leftSide * rightSide / shape;
        }

        public static string FormatTime(double totalSeconds)
        {
            int seconds = (int) (totalSeconds % 60);
            int minutes = (int) (totalSeconds / 60);
            int hours = minutes / 60;
            minutes = minutes % 60;

            var builder = new StringBuilder();

            if (hours > 0)
            {
                builder.Append(hours).Append(":")
                    .Append(minutes.ToString("00")).Append(":")
                    .Append(seconds.ToString("00"));
                return builder.ToString();
            }

            if (minutes > 0)
            {
                builder.Append(minutes).Append(":")
                    .Append(seconds.ToString("00"));
                return builder.ToString();
            }

            builder.Append(seconds).Append(" seconds");

            return builder.ToString();
        }

        public static BigInteger PowerFun(BigInteger baseValue, double root, int power)
        {
            return Tools.Multiply(baseValue, Math.Pow(root, power));
        }

        public static BigInteger PercentFun(BigInteger baseValue, float percent, int power)
        {
            return Tools.Multiply(baseValue, 1 + percent * power);
        }

        public static float SqrtFun(float startValue, float endValue, int gradeNumber, int level)
        {
            //value = b ^ level * startValue
            //b = power (endValue / startValue, 1 / maxLevel)

            var b = Mathf.Pow(endValue / startValue, 1 / (float) gradeNumber);
            return Mathf.Pow(b, level) * startValue;
        }

        public static int PercentFun(int baseValue, float percent, int power)
        {
            return (int) (baseValue * (1 + percent * power));
        }

        public static float PercentFun(float baseValue, float percent, int power)
        {
            return baseValue * (1 + percent * power);
        }
    }
}