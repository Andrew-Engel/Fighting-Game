using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace Tutorial.PatternCombo
{
    /// <summary>
    /// ***Factory class*** for creating combo-related gameplay action command objects.
    /// 
    /// This encapsulates the object creation process, allowing for easy addition
    /// and management of new gameplay action commands. By using a factory, we keep our
    /// system flexible and scalable, following the Open/Closed principle where the system
    /// is open for extension but closed for modification.
    /// </summary>
    public class ComboActionCommandFactory : MonoBehaviour
    {
        [SerializeField] Animator playerAnimator;
        [SerializeField] PlayerStateMachine playerStateMachine;

      
        public ICombatCommand CreateNewJabCrossCommand()
        {
            return new JabCrossCommand(playerAnimator, playerStateMachine);
        }
        public ICombatCommand CreateNewDodgeBackCommand()
        {
            return new DodgeBackCommand(playerAnimator, playerStateMachine, this.transform);
        }
        public ICombatCommand CreateNewDodgeLeftCommand()
        {
            return new DodgeLeftCommand(playerAnimator, playerStateMachine, this.transform);
        }
        public ICombatCommand CreateNewDodgeRightCommand()
        {
            return new DodgeRightCommand(playerAnimator, playerStateMachine, this.transform);
        }
        public ICombatCommand CreateNewChainPunchCommand()
        {
            return new ChainPunchCommand(playerAnimator,  playerStateMachine);
        }
        public ICombatCommand CreateNewIpponSeoiNageCommand()
        {
            return new IpponSeoiNageCommand(playerAnimator, playerStateMachine);
        }
        // Example of how to extend the factory with a new command:
        // public IGameplayActionCommand CreateSlideAttackCommand()
        // {
        //     return new SlideAttackActionCommand();
        // }
    }
}