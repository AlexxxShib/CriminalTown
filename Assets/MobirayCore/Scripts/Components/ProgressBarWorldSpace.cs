using System;
using UnityEngine;

namespace MobirayCore.Scripts.Components
{
    public class ProgressBarWorldSpace : MonoBehaviour
    {
        private MeshRenderer progressBar;
        private Material progressBarMat;
        
        private static readonly int FillPercent = Shader.PropertyToID("_fillPercent");

        private void Awake()
        {
            progressBar = GetComponent<MeshRenderer>();
            
            progressBarMat = new Material(progressBar.material);
            progressBar.material = progressBarMat;
            
            SetValue(0);
        }

        public void SetValue(float value)
        {
            progressBarMat.SetFloat(FillPercent, value);
        }
    }
}