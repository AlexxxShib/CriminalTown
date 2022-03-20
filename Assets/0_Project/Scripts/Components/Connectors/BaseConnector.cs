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
                    _isReady = value;

                    if (_isReady)
                    {
                        OnConnected?.Invoke(ConnectedObject);
                    }
                    else
                    {
                        OnDisconnected?.Invoke(ConnectedObject);
                    }
                }
            }
        }


        private CompHumanControl _humanControl;
        private ConfigMain _configMain;
        
        private float _currentTime;

        private MeshRenderer _progressRenderer;
        private Material _progressbarMat;
        private static readonly int FillPercent = Shader.PropertyToID("_fillPercent");

        protected virtual void Awake()
        {
            _humanControl = GetComponent<CompHumanControl>();
            
            _configMain = ToolBox.Get<ConfigMain>();
            
            _progressRenderer = progressBar.GetComponentInChildren<MeshRenderer>();
            _progressbarMat = new Material(_progressRenderer.material);
            _progressRenderer.material = _progressbarMat;

            _progressbarMat.SetFloat(FillPercent, 0);
        }
        
        private void Update()
        {
            if (!IsConnected)
            {
                return;
            }
            
            progressBar.rotation = Quaternion.Euler(0, 180, 0);

            if (_humanControl.IsMoving)
            {
                IsReady = false;
                
                _currentTime = 0;
                _progressbarMat.SetFloat(FillPercent, 0);
                return;
            }

            _currentTime += Time.deltaTime;
            
            _progressbarMat.SetFloat(FillPercent, Mathf.Clamp01(_currentTime / _configMain.connectionTime));
            
            // Debug.Log($"current time {_currentTime} from {_configMain.connectionTime}");

            if (_currentTime < _configMain.connectionTime)
            {
                return;
            }

            IsReady = true;
            _progressbarMat.SetFloat(FillPercent, 0);
        }

        public virtual void OnEnter(T connectedObject)
        {
            progressBar.gameObject.SetActive(true);
            _progressRenderer.material = _progressbarMat;
            
            IsConnected = true;
            IsReady = false;
            
            ConnectedObject = connectedObject;

            _currentTime = 0;
        }

        public virtual void OnExit(T connectedObject)
        {
            progressBar.gameObject.SetActive(false);
            
            IsConnected = false;
            IsReady = false;
            
            _progressbarMat.SetFloat(FillPercent, 0);
        }
    }
}