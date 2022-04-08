using CriminalTown.Configs;
using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class StreetFoodCrimeConnector : BaseCrimeConnector<EntityStreetFood>
    {
        protected override async void SetupCutscene(EntityStreetFood target)
        {
            cutscene = target.cutscene;

            ChangeReaction(_config.signalLastHit, OnLastHit);
            
            var salesman = target.salesman.transform;
            
            salesman.DOLookAt(transform.position, _config.lookAtTime);

            await transform.DOLookAt(salesman.position, _config.lookAtTime).AwaitFor();

            if (!IsReady)
            {
                return;
            }
            
            _player.SetupMoneyEmitter();
            
            var playableAsset = (TimelineAsset) cutscene.playableAsset;
            
            foreach (var outputTrack in playableAsset.GetOutputTracks())
            {
                if (cutsceneTrackMap.TryGetValue(outputTrack.name, out var value))
                {
                    cutscene.SetGenericBinding(outputTrack, value);
                }
            }

            // var trackKey = playableAsset.GetOutputTrack(cutsceneOuterTrackIndex);
            // cutscene.SetGenericBinding(trackKey, playerAnimator);
        }
    }
}