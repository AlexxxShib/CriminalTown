using System;
using UnityEngine;

namespace CriminalTown.Entities
{
    public class CompMeshProgress : MonoBehaviour
    {
        public string shaderValPercent = "_fillPercent";

        private Material _progressMat;

        private void Awake()
        {
            var renderer = GetComponentInChildren<MeshRenderer>(true);

            _progressMat = new Material(renderer.material);
            renderer.material = _progressMat;
            
            SetValue(0);
        }

        public void SetValue(float value)
        {
            _progressMat.SetFloat(shaderValPercent, Mathf.Clamp01(value));
        }
        
    }
}