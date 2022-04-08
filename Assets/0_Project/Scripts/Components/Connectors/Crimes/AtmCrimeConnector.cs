using CriminalTown.Entities;
using DG.Tweening;
using Mobiray.Common;
using UnityEngine.Timeline;

namespace CriminalTown.Components.Connectors
{
    public class AtmCrimeConnector : BaseCrimeConnector<EntityAtm>
    {

        protected override async void SetupCutscene(EntityAtm target)
        {
            cutscene = target.cutscene;
            
            ChangeReaction(_config.signalHit, OnHit);
            ChangeReaction(_config.signalLastHit, OnLastHit);
            
            await transform.DOLookAt(target.transform.position, _config.lookAtTime).AwaitFor();

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