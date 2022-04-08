using CriminalTown.Configs;
using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class CitizenCrimeConnector : BaseCrimeConnector<EntityCitizen>
    {
        public override bool OnEnter(EntityCitizen citizen)
        {
            if (base.OnEnter(citizen))
            {
                citizen.GetComponent<CompHumanControl>().InputEnabled = false;

                return true;
            }

            return false;
        }

        public override bool OnExit(EntityCitizen citizen)
        {
            if (!base.OnExit(citizen))
            {
                if (_victimHealth == null)
                {
                    Debug.LogError($"HEALTH NULL {citizen}");
                }
                
                if (!_victimHealth.Death)
                {
                    citizen.GetComponent<CompHumanControl>().InputEnabled = true;
                    citizen.SetPanic();
                }
            }

            return IsConnected;
        }
        
        protected override async void SetupCutscene(EntityCitizen target)
        {
            var lookAtDuration = ToolBox.Get<ConfigMain>().lookAtTime;
            
            target.transform.DOLookAt(transform.position, lookAtDuration);

            await transform.DOLookAt(target.transform.position, lookAtDuration).AwaitFor();

            if (!IsReady)
            {
                return;
            }
            
            // logger.LogDebug($"play cutscene, target {target.name} {_victimHealth}");

            var playableAsset = (TimelineAsset) cutscene.playableAsset;
            
            foreach (var outputTrack in playableAsset.GetOutputTracks())
            {
                if (outputTrack.name == "Citizen")
                {
                    cutscene.SetGenericBinding(outputTrack, target.GetComponentInChildren<Animator>());
                }
            }

            // var trackKey = playableAsset.GetOutputTrack(cutsceneOuterTrackIndex);
            // cutscene.SetGenericBinding(trackKey, target.GetComponentInChildren<Animator>());
            
            _player.SetupMoneyEmitter();
        }
    }
}