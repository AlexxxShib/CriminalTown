using System;
using System.Threading.Tasks;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using Mobiray.Common;
using Mobiray.Helpers;
using UnityEngine;

namespace CriminalTown.Entities
{
    public enum PanicMode
    {
        NONE, ACTIVE, PASSIVE
    }
    
    public struct SignalPlayerCaught { }
    
    public class EntityPolice : MonoBehaviour
    {
        public MobirayLogger logger;
        
        [Space]
        public ParticleSystem emojiQuestion;

        private ConfigMain _config;
        
        private CompMeshProgress _catchingProgressBar;
        private CompFieldOfView _searchFieldOfView;

        private EntityCitizen _citizen;
        private CompHumanControl _control;
        
        private EntityPlayer _player;
        private Transform _followAnchor;

        private PanicMode _panicMode;
        private float _catchingTime;
        
        private bool _prevIsPlayerVisible;
        private float _visibilityDelay = -1;

        private void Awake()
        {
            _config = ToolBox.Get<ConfigMain>();
            
            _catchingProgressBar = GetComponentInChildren<CompMeshProgress>();
            _catchingProgressBar.gameObject.SetActive(false);
            
            _searchFieldOfView = GetComponentInChildren<CompFieldOfView>();
            _searchFieldOfView.gameObject.SetActive(false);

            _citizen = GetComponent<EntityCitizen>();
            _citizen.OnPanicStarted += OnPanicStarted;

            _control = GetComponent<CompHumanControl>();
        }

        private void Start()
        {
            logger.mainTag = $"POLICE_{_citizen.logger.mainTag}";
            
            _player = ToolBox.Get<EntityPlayer>();
            
            _followAnchor = new GameObject($"anchor_{_citizen.logger.mainTag}").transform;
            _followAnchor.gameObject.AddComponent<GizmoSphereComp>().radius = 0.5f;
            _followAnchor.parent = transform;
            _followAnchor.localPosition = Vector3.zero;
        }

        private void Update()
        {
            if (_panicMode == PanicMode.NONE)
            {
                if (_player.CrimeInProgress)
                {
                    var playerDirection = _player.transform.position - transform.position;

                    if (playerDirection.magnitude < _searchFieldOfView.range)
                    {
                        SetPanicActive();
                    }
                }
                return;
            }

            var isPlayerVisible = PlayerIsVisible();

            if (!isPlayerVisible && _prevIsPlayerVisible)
            {
                _visibilityDelay = _config.policeVisibilityDelay;
            }
            _prevIsPlayerVisible = isPlayerVisible;
            _visibilityDelay -= Time.deltaTime;
            
            _player.hasPoliceVisor = isPlayerVisible; //TODO global value

            if (isPlayerVisible || _visibilityDelay > 0)
            {
                _followAnchor.position = _player.transform.position;
            }

            if (_panicMode == PanicMode.ACTIVE)
            {
                var sign = isPlayerVisible ? 1 : -1;
                _catchingTime += sign * Time.deltaTime;
                
                _catchingProgressBar.SetValue(_catchingTime / _config.policeCatchTime);

                if (_catchingTime >= _config.policeCatchTime)
                {
                    PlayerIsCaught();
                    return;
                }
            }

            if (_panicMode == PanicMode.PASSIVE)
            {
                if (isPlayerVisible)
                {
                    SetPanicActive();
                    return;
                }

                _catchingTime += Time.deltaTime;
                if (_catchingTime >= _config.policePassiveTime) //TODO global
                {
                    DisablePanic();
                    
                    ToolBox.Get<CitizenSystem>().ReturnPolice(_citizen, _control);
                }
            }
        }

        private void OnPanicStarted()
        {
            SetPanicActive();
        }

        public void SetPanicActive()
        {
            if (_panicMode == PanicMode.ACTIVE)
            {
                return;
            }
            
            _panicMode = PanicMode.ACTIVE;
            _followAnchor.parent = transform.parent;
            
            // _catchingTime = _config.policeCatchTime / 2;
            _catchingTime = 0;
            
            _catchingProgressBar.gameObject.SetActive(true);
            _searchFieldOfView.gameObject.SetActive(true);

            _control.MaxSpeed = _config.citizenSpeedRun;
            _control.SetDestination(_followAnchor, OnFoundPlayer);
        }

        public async void SetPanicPassive()
        {
            _panicMode = PanicMode.PASSIVE;
            _catchingTime = 0;
            
            _catchingProgressBar.gameObject.SetActive(false);
            _control.MaxSpeed = _config.citizenSpeedWalk;
            
            // ToolBox.Get<CitizenSystem>().ReturnPolice(_citizen, _control);
            emojiQuestion.Play();
            
            await Task.Delay(TimeSpan.FromSeconds(2f));

            if (_panicMode != PanicMode.PASSIVE)
            {
                return;
            }
            
            _followAnchor.position = _followAnchor.position.RandomPoint(3f);
            _control.SetDestination(_followAnchor, OnFoundPlayer);
        }

        public void DisablePanic()
        {
            _panicMode = PanicMode.NONE;
            _citizen.Panic = false;
            
            _searchFieldOfView.gameObject.SetActive(false);
            _catchingProgressBar.gameObject.SetActive(false);
            
            _control.MaxSpeed = _config.citizenSpeedWalk;
            
            _followAnchor.parent = transform;
            
            // ToolBox.Get<CitizenSystem>().ReturnPolice(_citizen, _control);
        }

        private void PlayerIsCaught()
        {
            DisablePanic();

            _control.InputEnabled = false;
            _player.Catch();
        }

        private bool PlayerIsVisible()
        {
            return !_player.isHidden && _searchFieldOfView.IsVisible(_player.transform);
        }

        private async void OnFoundPlayer(CompHumanControl control)
        {
            if (!PlayerIsVisible() && _visibilityDelay < 0)
            {
                SetPanicPassive();
                return;
            }
            
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            control.SetDestination(_followAnchor, OnFoundPlayer);
        }
    }
}