using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mobiray.Common;
using UnityEngine;

namespace Mobiray.Movement
{
    
    [RequireComponent(typeof(NavMeshFollowController))]
    public class FollowPathController : MonoBehaviour
    {

        public List<Transform> Path = new List<Transform>();
        public List<float> PointDelays = new List<float>();
        
        [Space]
        public bool Loop;
        public bool WaitOnPoints;

        public Action ObjectOnPosition;

        private NavMeshFollowController followController;

        private int currentPoint = 0;

        private void Awake()
        {
            followController = GetComponent<NavMeshFollowController>();
            followController.Init();
        }

        private void OnEnable()
        {
            if (Path.Count > 0) GoToCurrentPos();
        }

        public void SetPath(List<Transform> path, float delay = -1)
        {
            Path.Clear();
            Path.AddRange(path);

            currentPoint = 0;

            if (path.Count == 0)
            {
                Debug.LogError("Path is empty");
                return;
            }

            if (delay > 0)
            {
                this.StartTimer(delay, GoToCurrentPos);
            }
            else
            {
                GoToCurrentPos();
            }
        }

        public void SetPath(Transform pathParent, float delay = -1)
        {
            var path = new List<Transform>();
            for (int i = 0; i < pathParent.childCount; i++)
            {
                path.Add(pathParent.GetChild(i));
            }
            
            SetPath(path, delay);
        }

        public void ForgetPath()
        {
            Path.Clear();
            currentPoint = 0;
            
            followController.ForgetFollowObject();
        }

        private void GoToCurrentPos()
        {
            // Debug.Log($"GoToCurrentPos {currentPoint}");
            
            followController.ManOnPoint += GoToNextPos;
            followController.SetFollowObject(Path[currentPoint]);
        }

        private async void GoToNextPos()
        {
            // Debug.Log("GoToNextPos");
            
            if (currentPoint < PointDelays.Count && WaitOnPoints)
            {
                await Task.Delay(TimeSpan.FromSeconds(PointDelays[currentPoint]));
            }

            currentPoint++;
            if (currentPoint >= Path.Count)
            {
                if (!Loop || Path.Count == 1) return;

                currentPoint = 0;
                
                ObjectOnPosition?.Invoke();
            }
            
            GoToCurrentPos();
        }
        
    }
}