using System;
using UnityEngine;
using UnityEngine.AI;

namespace Mobiray.Movement
{
    [RequireComponent(typeof(AbsoluteMovementController))]
    [RequireComponent(typeof(CustomJoystick))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshFollowController : FollowController
    {
        public NavMeshAgent Agent;

        public Action ManOnPoint;

        public bool ShowDebugLogs = false;

        public void SetFollowObject(Transform followObject)
        {
            base.SetFollowObject(followObject);
        }

        public override void Init()
        {
            Agent = GetComponent<NavMeshAgent>();
            base.Init();
        }

        void OnDrawGizmos()
        {
            try
            {
                if (!Agent.hasPath) return;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, Agent.path.corners[0]);
                for (int i = 0; i < Agent.path.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(Agent.path.corners[i], Agent.path.corners[i + 1]);
                }
            }
            catch (Exception e)
            {
            }
        }
        
        private void Update()
        {
            if (!KnowFollowObject) return;

            Agent.SetDestination(FollowObject.transform.position);
            Vector3 dir = Vector3.zero;

            if ((transform.position - FollowObject.transform.position).magnitude < 0.5f && ManOnPoint != null)
            {
                ManOnPoint.Invoke();
                ManOnPoint = null;
            }

            if (Agent.path.corners.Length >= 2 && Agent.remainingDistance > Agent.stoppingDistance)
            {
                dir = Agent.path.corners[1] - Agent.path.corners[0];
                if (Agent.path.corners.Length > 2)
                {
                    dir = dir.normalized;
                }

                if (dir.magnitude < Agent.stoppingDistance && Agent.path.corners.Length > 2)
                {
                    dir = Agent.path.corners[2] - Agent.path.corners[1];
                }

                if (dir.magnitude > 1)
                {
                    dir = dir.normalized;
                }
            }
            
            if (ShowDebugLogs)
            {
                Debug.Log(
                    $"dir: {dir}; corners.len: {Agent.path.corners.Length}; remDist: {Agent.remainingDistance}; mag: {dir.magnitude}");
            }

            customJoystick.UpdateDirection(dir);

            /* if (Agent.remainingDistance > Agent.stoppingDistance &&
                 customJoystick.UpdateDirection(2 * (Agent.path.corners[1] - Agent.path.corners[0]).normalized))
             {
                 ManOnPoint?.Invoke();
                 ManOnPoint = null;
             }
             else if (Agent.remainingDistance <= Agent.stoppingDistance)
             {
                 customJoystick.UpdateDirection(0, 0);
             }*/
        }
    }
}