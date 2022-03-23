using System;
using System.Collections.Generic;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class CompFieldOfView : MonoBehaviour
    {
        [Header("Runtime")]
        public float range;
        public float angle;

        [Space]
        public Action<Transform> OnInsideRange;
        public List<Transform> searchObjects = new();

        private static readonly int FillPercent = Shader.PropertyToID("_fillPercent");
        
        private void Awake()
        {
            var mesh = GetComponentInChildren<MeshRenderer>();

            range = mesh.transform.localScale.x / 2;

            var mat = mesh.sharedMaterial;
            angle = mat.GetFloat(FillPercent) * 360f;
        }

        private void Update()
        {
            foreach (var searchObject in searchObjects)
            {
                if (IsVisible(searchObject))
                {
                    OnInsideRange?.Invoke(searchObject);
                }
            }
        }

        public bool IsVisible(Transform obj)
        {
            var direction = obj.position - transform.position;
            if (direction.magnitude > range) return false;

            var dot = Vector3.Dot(direction.normalized, transform.forward);
            var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            return angle < this.angle / 2;
        }
    }
}