using UnityEngine;
using System.Collections.Generic;
using System.Windows.Input;
using Tutorial.PatternCombo;


    public class AttackInvoker
    {
         ComboActionQueueManager _combatQueManager;
        Stack<ICombatCommand> _attackList;

        public AttackInvoker(ComboActionQueueManager combatQueManager)
        {
            _attackList = new Stack<ICombatCommand>();
        _combatQueManager = combatQueManager;
        }
        public void AddCommand(ICombatCommand newAttack)
        {
            newAttack.Execute();
            _combatQueManager.AddCommandToComboSequence(newAttack);
          //  _attackList.Push(newAttack);
        }


    }

