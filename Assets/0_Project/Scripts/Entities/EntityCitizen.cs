using System;
using CriminalTown.Components.Connectors;
using CriminalTown.Configs;
using Mobiray.Common;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CriminalTown.Entities
{
    public class EntityCitizen : MonoBehaviour
    {
        public MobirayLogger logger;
        public bool staff;
        
        [Space]
        public Transform bodyContainer;
        public Transform headContainer;
        public Transform glassesContainer;

        [Space]
        public ParticleSystem emojiShocked;

        [Space]
        public int reward = 100;
        
        public int Health { get; private set; }
        public bool Death { get; private set; }

        private bool _panic;

        public bool Panic
        {
            get => _panic;
            set
            {
                if (value != _panic)
                {
                    _panic = value;
                    
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

        private EntityPlayer _player;
        private CitizenConnector _connector;

        private void Awake()
        {
            _config = ToolBox.Get<ConfigMain>();

            Health = _config.citizenHealth;
            
            InitializeView();

            _humanControl = GetComponent<CompHumanControl>();
            _humanControl.MaxSpeed = _config.citizenSpeedWalk;
        }

        private void Start()
        {
            _player = ToolBox.Get<EntityPlayer>();
        }

        private void Update()
        {
            TrySetPanic();
        }

        private void TrySetPanic()
        {
            if (Panic || Death)
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

        private void OnTriggerEnter(Collider other)
        {
            if (Death || staff)
            {
                return;
            }
            
            _connector = other.GetComponentInParent<CitizenConnector>();

            if (_connector != null)
            {
                _connector.OnEnter(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var connector = other.GetComponentInParent<CitizenConnector>();

            if (connector != null)
            {
                connector.OnExit(this);
            }
            
            if (!Death && Health <= 0)
            {
                Death = true;

                _humanControl.SetDeath();
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

        public bool ApplyHit(bool lastHit)
        {
            var hasHealth = Health > 0;

            if (hasHealth)
            {
                Health--;
            }

            if (lastHit && Health <= 0)
            {
                Death = true;

                _humanControl.SetDeath();
            }

            return hasHealth;
        }

        [Button("Panic")]
        public void SetPanic()
        {
            if (Death || Panic)
            {
                return;
            }
            
            Panic = true;
            
            _humanControl.MaxSpeed = _config.citizenSpeedRun + 0.1f;
            
            emojiShocked.Play();
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