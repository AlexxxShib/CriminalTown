using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using Mobiray.Helpers;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
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
            if (!mono.gameObject.activeInHierarchy) return;
            
            mono.StartCoroutine(StartTimer(time, action));
        }

        private static IEnumerator StartTimer(float time, Action action)
        {
            // yield return new WaitForSeconds(time);
            
            var start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time)
            {
                yield return null;
            }

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

        public static Vector3 GetCenterPoint(List<Vector3> points, bool nullifyY)
        {
            if (points.Count == 0) return Vector3.zero;
            
            Vector3 result = Vector3.zero;
            foreach (var point in points)
            {
                result += point;
            }

            if (nullifyY) result.y = 0;

            return result / points.Count;
        }
        
        public static Vector3 GetCenterPoint(List<Transform> points, bool nullifyY)
        {
            if (points.Count == 0) return Vector3.zero;
            
            var vectors = points.ConvertAll(t => t.position);

            return GetCenterPoint(vectors, nullifyY);
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

        public static void DestroyChildren(this Transform parent)
        {
            var children = parent.GetChildren();
            foreach (var transform in children)
            {
                GameObject.Destroy(transform.gameObject);
            }
        }

        public static List<Transform> GetChildren(this Transform parent)
        {
            var list = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
            {
                list.Add(parent.GetChild(i));
            }

            return list;
        }

        public static List<GameObject> FindChildrenWithTag(this GameObject gameObject, string tag)
        {
            var result = new List<GameObject>();
            FindChildrenWithTag(gameObject, tag, result);
            return result;
        }

        private static void FindChildrenWithTag(GameObject gameObject, string tag, ICollection<GameObject> result)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i).gameObject;
                
                if (child.CompareTag(tag))
                {
                    result.Add(child);
                }

                FindChildrenWithTag(child, tag, result);
            }
        }
        
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach(Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }    

        public static void SetSame(this Transform transform, Transform original, bool withScale = false)
        {
            transform.position = original.position;
            transform.rotation = original.rotation;

            if (withScale) transform.localScale = original.localScale;
        }

        public static void SetSame(this Transform transform, Transform original, float smoothTime)
        {
            transform.DOMove(original.position, smoothTime);
            transform.DORotate(original.rotation.eulerAngles, smoothTime);
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

        public static int RandomIndexByWeights<T>(this List<T> list, List<float> weights)
        {
            if (list.Count != weights.Count)
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
                    return i;
                }

                counter += weights[i];
            }

            return list.Count - 1;
        }

        public static T RandomItemByWeights<T>(this List<T> list, IEnumerable<float> weights)
        {
            var index = RandomIndexByWeights(list, weights.ToList());
            return list[index];
        }

        public static T RandomItemByWeights<T>(this List<T> list, List<float> weights)
        {
            var index = RandomIndexByWeights(list, weights);
            return list[index];
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
        
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector2.Lerp(start, end, t);

            return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
        }

        public static async Task<bool> CheckInternet()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }
            
            var time = 0f;
            var timeout = 5f;

            var result = false;

            try
            {
                var ping = new Ping("8.8.8.8");

                while (!ping.isDone && time < timeout)
                {
                    await Task.Yield();
                    time += Time.deltaTime;
                }

                result = ping.isDone;
                ping.DestroyPing();

            } catch (Exception _) { }

            return result;
        }
    }
}