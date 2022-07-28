using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using Mobiray.Helpers;
using MobirayCore.Helpers;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Mobiray.Common
{
    public static class Tools
    {

        public static bool IsVisible(this Camera camera, Renderer renderer)
        {
            return camera.IsVisible(renderer.bounds);
        }
        
        public static bool IsVisible(this Camera camera, Bounds bounds)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
        
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

        public static void ChangeRotX(this Transform transform, float x)
        {
            var rotation = transform.rotation.eulerAngles;

            rotation.x = x;
            transform.rotation = Quaternion.Euler(rotation);
        }
        
        public static void ChangeRotY(this Transform transform, float y)
        {
            var rotation = transform.rotation.eulerAngles;

            rotation.y = y;
            transform.rotation = Quaternion.Euler(rotation);
        }
        
        public static void ChangeRotZ(this Transform transform, float z)
        {
            var rotation = transform.rotation.eulerAngles;

            rotation.z = z;
            transform.rotation = Quaternion.Euler(rotation);
        }

        public static Vector3 ToVector(this float value)
        {
            return new Vector3(value, value, value);
        }

        public static Vector3 RandomPoint(this Vector3 center, float radius)
        {
            return GetRandomPointInArea(center, radius);
        }

        public static Vector3 GetRandomPointInArea(Vector3 center, float radius)
        {
            float randomAngle = Random.Range(0f, 2 * Mathf.PI);
            float randomRadius = Random.Range(0f, radius);

            return center + new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * randomRadius;
        }

        public static Vector3 GetRandomPointInArea(this GizmoSphereComp gizmo)
        {
            return GetRandomPointInArea(gizmo.transform.position, gizmo.radius);
        }

        public static Vector3 RandomRange(this Vector3 vector, Vector3 range)
        {
            /*return new Vector3(
                Random.Range(vector.x, range.x),
                Random.Range(vector.y, range.y),
                Random.Range(vector.z, range.z)
            );*/

            return new Vector3(
                vector.x + Random.Range(0, range.x),
                vector.y + Random.Range(0, range.y),
                vector.z + Random.Range(0, range.z)
            );
        }

        public static Vector3 GetCenterPoint(IEnumerable<Vector3> points, bool nullifyY = false)
        {
            int count = 0;
            Vector3 result = Vector3.zero;
            
            foreach (var point in points)
            {
                result += point;
                count++;
            }
            
            if (count == 0) return Vector3.zero;

            if (nullifyY) result.y = 0;

            return result / count;
        }
        
        public static Vector3 GetCenterPoint(List<Transform> points, bool nullifyY = false)
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
                GameObject.DestroyImmediate(transform.gameObject);
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

        public static bool TryGetComponentInParentChildren<T>(this Transform transform, out T result, 
            int startLevel = 0, int maxLevel = 5) where T : MonoBehaviour
        {
            var parent = transform.parent;
            result = null;
            
            for (int i = 0; i < maxLevel; i++)
            {
                if (i < startLevel) continue;
                
                if (parent == null) return false;
                
                result = parent.GetComponentInChildren<T>();

                if (result != null) return true;

                parent = parent.parent;
            }

            return false;
        }

        public static void ActiveChild(this Transform transform, int childIndex)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == childIndex);
            }
        }
        
        public static void ActiveToChild(this Transform transform, int childIndex, bool include = true)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var active = i < childIndex || include && i == childIndex;
                
                transform.GetChild(i).gameObject.SetActive(active);
            }
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

        public static bool TryFindNearest<T>(Vector3 position, IEnumerable<T> objects, out T result) where T : class
        {
            var minDistance = float.MaxValue;
            result = default;

            var found = false;
            
            foreach (var obj in objects)
            {
                var transform = obj as Transform;
                if (transform == null)
                {
                    transform = (obj as MonoBehaviour).transform;
                }
                
                var distance = (transform.position - position).magnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = obj;

                    found = true;
                }
            }

            return found;
        }

        public static bool TryFindNearest(Transform point, IEnumerable<Transform> objects, out Transform result, 
            bool behindOnly = false)
        {
            var minDistance = float.MaxValue;
            result = default;

            var found = false;
            
            foreach (var obj in objects)
            {
                var direction = obj.transform.position - point.position;
                var distance = direction.magnitude;
                
                if (behindOnly)
                {
                    var angle = Vector3.SignedAngle(direction, point.forward, Vector3.up);
                    if (Mathf.Abs(angle) < 90) continue;
                }

                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = obj;

                    found = true;
                }
            }

            return found;
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

        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(0, list.Count);

                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }

            return list;
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
                (sb[i], sb[j]) = (sb[j], sb[i]);
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
        
        public static async Task<T> AwaitFor<T>(this T t) where T : Tween {
            var completionSource = new TaskCompletionSource<T>();
            t.OnComplete(() => completionSource.SetResult(t));
            return await completionSource.Task;
        }

        public static Tween TweenTimeScale(float to, float duration)
        {
            return DOTween.To(() => Time.timeScale, x => Time.timeScale = x,
                to, duration);
        }

        public static Material ChangeColor(this Material material, Color color)
        {
            var copy = new Material(material);

            copy.color = color;

            return copy;
        }

        public static void Recolor(this GameObject go, HashMap<Material, Material> colorMap)
        {
            var renderers = go.GetComponentsInChildren<MeshRenderer>();

            foreach (var meshRenderer in renderers)
            {
                var materials = meshRenderer.sharedMaterials;
                var changedColor = false;

                for (var index = 0; index < materials.Length; index++)
                {
                    var material = materials[index];
                    if (colorMap.TryGetValue(material, out var projection))
                    {
                        materials[index] = projection;
                        changedColor = true;
                    }
                }

                if (changedColor)
                {
                    meshRenderer.materials = materials;
                }
            }
        }

        public static void ChangeColor(this MeshRenderer renderer, Color color)
        {
            var newMat = renderer.material.ChangeColor(color);

            renderer.material = newMat;
        }
        
        public static void ChangeColor(this ParticleSystemRenderer renderer, Color color)
        {
            var newMat = renderer.material.ChangeColor(color);

            renderer.material = newMat;
        }

        public static int ToInt(this bool value) { return value ? 1 : 0; }
    }
}