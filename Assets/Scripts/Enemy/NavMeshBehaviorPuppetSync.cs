using RootMotion.Dynamics;
using UnityEngine;

public class NavMeshBehaviorPuppetSync : MonoBehaviour
{
    public BehaviourPuppet puppet;
    public UnityEngine.AI.NavMeshAgent agent;

    void Update()
    {
        // Keep the agent disabled while the puppet is unbalanced.
        agent.enabled = puppet.state == BehaviourPuppet.State.Puppet;

     
    }
}
