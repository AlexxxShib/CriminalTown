using System;
using CriminalTown.Configs;
using CriminalTown.Entities;
using Mobiray.Common;
using UnityEngine;

namespace CriminalTown.Components.Connectors
{
    public abstract class BaseConnector<T> : MonoBehaviour where T : MonoBehaviour
    {
        public Transform progressBar;

        public Action<T> OnConnected;
        public Action<T> OnDisconnected;

        public bool IsConnected { get; protected set; }
        public T ConnectedObject { get; protected set; }

        private bool _isReady;

        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (value != _isReady)
                {
                    if (value)
                    {
                        OnConnected?.Invoke(ConnectedObject);
                    }
                    else
                    {
                        OnDisconnected?.Invoke(ConnectedObject);
                    }
                }

                _isReady = value;
            }
        }


        private CompHumanControl _humanControl;
        private ConfigMain _configMain;
        
        private float _currentTime;
        
        private Material _progressbarMat;
        private static readonly int FillPercent = Shader.PropertyToID("_fillPercent");

        private void Awake()
        {
            _humanControl = GetComponent<CompHumanControl>();
            
            _configMain = ToolBox.Get<ConfigMain>();
            
            var progressRenderer = progressBar.GetComponentInChildren<MeshRenderer>();
            _progressbarMat = new Material(progressRenderer.material);
            progressRenderer.material = _progressbarMat;
            _progressbarMat.SetFloat(FillPercent, 0);
        }
        
        private void Update()
        {
            progressBar.rotation = Quaternion.Euler(0, 180, 0);

            if (!IsConnected)
            {
                return;
            }

            if (_humanControl.IsMoving)
            {
                IsReady = false;
                
                _currentTime = 0;
                _progressbarMat.SetFloat(FillPercent, 0);
                return;
            }

            _currentTime += Time.deltaTime;
            
            _progressbarMat.SetFloat(FillPercent, Mathf.Clamp01(_currentTime / _configMain.connectionTime));

            if (_currentTime < _configMain.connectionTime)
            {
                return;
            }

            IsReady = true;
        }

        public void OnEnter(T connectedObject)
        {
            IsConnected = true;
            IsReady = false;
            
            ConnectedObject = connectedObject;

            _currentTime = 0;
        }

        public void OnExit(T connectedObject)
        {
            IsConnected = false;
            IsReady = false;
            
            _progressbarMat.SetFloat(FillPercent, 0);
        }
    }
}