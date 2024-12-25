using RootMotion.Dynamics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CutSceneModuleBehavior : MonoBehaviour
{
    public SightTargetBehavior enemyObject;
    PuppetMaster puppetMaster;
    public PlayableDirector playableDirector;
    public string trackName;
    PlayerStateMachine _playerStateMachine;
    GameObject toggleObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerStateMachine = GetComponentInParent<PlayerStateMachine>();
    }

public void AssignEnemy()
    {
        enemyObject = _playerStateMachine.nearbyEnemies[_playerStateMachine.enemyFacingIndex].gameObject.GetComponent<SightTargetBehavior>();
      if (enemyObject.hasPuppet)
        {
            puppetMaster = enemyObject.puppet;
        }
        toggleObject = enemyObject.parentObject;
        var timelineAsset = (TimelineAsset)playableDirector.playableAsset;
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                // Bind the GameObject to the track
                playableDirector.SetGenericBinding(track, toggleObject);
                break;
            }
        }
    }
    public void KillEnemy()
    {
        puppetMaster.state = PuppetMaster.State.Dead;
    }
  
}
