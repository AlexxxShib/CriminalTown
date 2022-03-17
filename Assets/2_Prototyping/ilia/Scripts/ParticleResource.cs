using System;
using System.Collections.Generic;
using Mobiray.Common;
using Template.Configs;
using Template.Controllers;
using Template.Data;
using UnityEngine;
using UnityEngine.Events;

namespace _0_Project.Scripts
{
    public class ParticleResource : MonoBehaviour
    {
        public ParticleSystem ResourceParticleSystem;
        public ResType ResType;
        public int ResourceCount;

        [HideInInspector] public UnityEvent onResourcesCollected;

        private DataGameState _gameState;
        private ConfigMain _configMain;
        private ConfigTaptic _configTaptic;

        private int _triggeredParticles;
        
        private void Start()
        {
            _configMain = ToolBox.Get<ConfigMain>();
            _gameState = ToolBox.Get<DataGameState>();
            _configTaptic = ToolBox.Get<ConfigTaptic>();
            onResourcesCollected.AddListener(() => SoundController.PlayHaptic(_configTaptic.haptics["resourceGet"]));
        }

        public void MakeRenewable()
        {
            onResourcesCollected.AddListener(ResetResource);
        }

        private void OnParticleTrigger()
        {
            var particles = new List<ParticleSystem.Particle>();
            ResourceParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, particles);

            for (var i = 0; i < particles.Count; i++)
            {
                switch (ResType)
                {
                    case ResType.Gold:
                        _gameState.GoldTransaction(1);
                        break;
                    default:
                        _triggeredParticles++;
                        break;
                }

                var particle = particles[i];
                particle.remainingLifetime = 0;
                particles[i] = particle;
            }
            
            ResourceParticleSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, particles);

            if (_triggeredParticles - ResourceParticleSystem.emission.GetBurst(0).count.constant >= 0 && enabled) {
                switch (ResType)
                {
                    case ResType.Food:
                        _gameState.FoodTransaction(ResourceCount);
                        break;
                    case ResType.Skins:
                        _gameState.SkinsTransaction(ResourceCount);
                        break;
                    case ResType.Wood:
                        _gameState.WoodTransaction(ResourceCount);
                        break;
                }
                
                ResourceParticleSystem.Stop();
                
                enabled = false;
                onResourcesCollected.Invoke();
            }
        }

        public void ResetResource()
        {
            _triggeredParticles = 0;
            enabled = true;
        }
    }
}
