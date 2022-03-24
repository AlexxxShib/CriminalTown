using System;
using System.Threading.Tasks;
using CriminalTown.Configs;
using Mobiray.Common;
using Mobiray.Helpers;
using UnityEngine;

namespace CriminalTown.Entities
{
    public enum PanicMode
    {
        NONE, ACTIVE, PASSIVE
    }
    
    public class EntityPolice : SignalReceiver
    {
        public MobirayLogger logger;

        [Space]
        public CompFieldOfView searchFieldOfView;
        public CompFieldOfView catchingFieldProgress;

        [Space]
        public GameObject weapon;
        public ParticleSystem emojiQuestion;

        public bool SawPlayer => _prevIsPlayerVisible;

        private ConfigMain _config;
        
        // private CompMeshProgress _catchingProgressBar;

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
            
            searchFieldOfView.gameObject.SetActive(false);
            catchingFieldProgress.gameObject.SetActive(false);

            _citizen = GetComponent<EntityCitizen>();
            _citizen.OnPanicStarted += OnPanicStarted;

            _control = GetComponent<CompHumanControl>();
            
            weapon.SetActive(false);
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

                    if (playerDirection.magnitude < searchFieldOfView.range)
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
            
            if (isPlayerVisible || _visibilityDelay > 0)
            {
                _followAnchor.position = _player.transform.position;
            }

            if (_panicMode == PanicMode.ACTIVE)
            {
                var sign = isPlayerVisible ? 1 : -1;
                _catchingTime += sign * Time.deltaTime;

                var progress = Mathf.Clamp01(_catchingTime / _config.policeCatchTime);
                
                // _catchingProgressBar.SetValue(_catchingTime / _config.policeCatchTime);
                catchingFieldProgress.transform.localScale = progress.ToVector().ChangeY(1);

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
            
            weapon.SetActive(true);
            
            ToolBox.Signals.Send<SignalPoliceActivated>();
            
            _panicMode = PanicMode.ACTIVE;
            _followAnchor.parent = transform.parent;
            _followAnchor.position = _player.transform.position;
            
            _catchingTime = 0;
            _visibilityDelay = _config.policeVisibilityDelay;
            
            catchingFieldProgress.gameObject.SetActive(true);
            searchFieldOfView.gameObject.SetActive(true);

            _control.MaxSpeed = _config.citizenSpeedRun;
            _control.SetDestination(_followAnchor, OnFoundPlayer);
        }

        public async void SetPanicPassive()
        {
            _panicMode = PanicMode.PASSIVE;
            _catchingTime = 0;
            
            catchingFieldProgress.gameObject.SetActive(false);
            _control.MaxSpeed = _config.citizenSpeedWalk;
            
            // ToolBox.Get<CitizenSystem>().ReturnPolice(_citizen, _control);
            emojiQuestion.Play();

            _control.InputEnabled = false;
            
            await Task.Delay(TimeSpan.FromSeconds(2f));
            
            _control.InputEnabled = true;

            if (_panicMode == PanicMode.PASSIVE)
            {
                _followAnchor.position = _followAnchor.position.RandomPoint(3f);
                _control.SetDestination(_followAnchor, OnFoundPlayer);
            }
        }

        public void DisablePanic()
        {
            if (_panicMode == PanicMode.NONE)
            {
                return;
            }
            
            _panicMode = PanicMode.NONE;
            _citizen.Panic = false;
            
            searchFieldOfView.gameObject.SetActive(false);
            catchingFieldProgress.gameObject.SetActive(false);
            
            _control.MaxSpeed = _config.citizenSpeedWalk;
            
            _followAnchor.parent = transform;
            _followAnchor.localPosition = Vector3.zero;
        }

        private void PlayerIsCaught()
        {
            DisablePanic();
            
            _control.SetPistol(true);
            
            _player.Catch();
        }

        private bool PlayerIsVisible()
        {
            return !_player.isHidden && searchFieldOfView.IsVisible(_player.transform);
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