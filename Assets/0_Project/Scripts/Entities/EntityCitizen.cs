using System;
using CriminalTown.Components;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using CriminalTown.Controllers;
using DG.Tweening;
using Mobiray.Common;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CriminalTown.Entities
{

    public struct SignalCitizenPanic
    {
        public bool activated;
        public EntityCitizen citizen;

        public static SignalCitizenPanic Activate(EntityCitizen citizen) => new() {activated = true, citizen = citizen};
        
        public static SignalCitizenPanic Deactivate(EntityCitizen citizen) => new() {activated = false, citizen = citizen};
    } 
    
    public class EntityCitizen : BaseConnectorTrigger<EntityCitizen, CitizenCrimeConnector>
    {
        public bool staff;
        
        [Space]
        public Transform bodyContainer;
        public Transform headContainer;
        public Transform glassesContainer;

        [Space]
        public GameObject panicSign;
        public ParticleSystem emojiShocked;
        
        private bool _panic;
        private float _deathTime;

        public bool Snitch { get; private set; }

        public bool Death => _health.Death;

        public bool Panic
        {
            get => _panic;
            set
            {
                if (value != _panic)
                {
                    _panic = value;

                    if (panicSign != null)
                    {
                        panicSign.SetActive(_panic);
                    }
                    
                    if (_panic)
                    {
                        OnPanicStarted?.Invoke();
                    }
                    else
                    {
                        OnPanicFinished?.Invoke();
                    }
                }
            }
        }

        public Action OnPanicStarted;
        public Action OnPanicFinished;

        private ConfigMain _config;

        private CompHumanControl _humanControl;
        private CompHealth _health;

        private EntityPlayer _player;

        protected override void Awake()
        {
            base.Awake();
            
            _config = ToolBox.Get<ConfigMain>();

            InitializeView();

            _humanControl = GetComponent<CompHumanControl>();
            _humanControl.MaxSpeed = _config.citizenSpeedWalk;

            _health = GetComponent<CompHealth>();
            _health.OnDeath += OnDeath;

            available = !staff;

            if (panicSign != null)
            {
                panicSign.SetActive(false);
            }
        }

        private void Start()
        {
            _player = ToolBox.Get<EntityPlayer>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            ToolBox.Signals.Send(SignalCitizenPanic.Deactivate(this));
        }

        private void Update()
        {
            if (_health.Death)
            {
                if (_deathTime > 0)
                {
                    _deathTime -= Time.deltaTime;

                    if (_deathTime <= 0)
                    {
                        var destPoint = transform.position + Vector3.down;

                        transform.DOMove(destPoint, 1.5f).OnComplete(() =>
                        {
                            ToolBox.Signals.Send(SignalTryDestroyCitizen.Destroy(this));
                        });
                    }
                }
                return;
            }
            
            TrySetPanic();
        }

        private void TrySetPanic()
        {
            if (Panic || _health.Death)
            {
                return;
            }
            
            if (_connector != null && _connector.IsItConnected(this))
            {
                return;
            }
            
            var playerDirection = _player.transform.position - transform.position;
            if (playerDirection.magnitude <= _config.citizenPanicDistance)
            {
                var dot = Vector3.Dot(playerDirection.normalized, transform.forward);
                var angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle < _config.citizenPanicAngle / 2 && _player.CrimeInProgress)
                {
                    SetPanic();
                }
            }
        }

        private void InitializeView()
        {
            var material = _config.citizenMaterials.RandomItem();
            
            var body = SetRandomChild(bodyContainer, material, 1);
            
            var name = body.name.Replace("Body_", String.Empty);
            
            gameObject.name = name;
            logger.mainTag = name;
            
            SetRandomChild(headContainer, material);

            var hasGlasses = 0.1f.Chance();
            
            glassesContainer.gameObject.SetActive(hasGlasses);

            if (hasGlasses)
            {
                SetRandomChild(glassesContainer, material);
            }
        }

        [Button("Panic")]
        public void SetPanic()
        {
            if (_health.Death || Panic)
            {
                return;
            }
            
            Panic = true;

            if (!ToolBox.Get<CitizenSystem>().PoliceActivated)
            {
                Snitch = true;
                
                ToolBox.Signals.Send(SignalCitizenPanic.Activate(this));
            }
            
            _humanControl.MaxSpeed = _config.citizenSpeedRun + 0.1f;
            
            emojiShocked.Play();
        }

        private void OnDeath()
        {
            _deathTime = _config.citizenDeathTime;
            
            _humanControl.SetDeath();

            Panic = false;
            Snitch = false;
            
            ToolBox.Signals.Send(SignalCitizenPanic.Deactivate(this));
        }

        private static GameObject SetRandomChild(Transform container, Material material, int from = 0)
        {
            var variant = Random.Range(from, container.childCount);
            GameObject variantGo = null;
            
            for (int i = from; i < container.childCount; i++)
            {
                var go = container.GetChild(i).gameObject;
                
                go.SetActive(i == variant);

                if (i == variant)
                {
                    go.GetComponent<Renderer>().material = material;

                    variantGo = go;
                }
            }

            return variantGo;
        }
    }
}