using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tutorial.PatternCombo;


    public class DodgeBack : IComboRule
    {
        private ComboActionCommandFactory _comboActionCommandFactory;

        public int ComboLength { get; private set; }

        public DodgeBack(ComboActionCommandFactory actionCommandFactory)
        {
            _comboActionCommandFactory = actionCommandFactory;
            ComboLength = 2;
        }

        public bool IsFirstConditionMet(ICombatCommand firstCommand)
        {
            return firstCommand is BackInputCommand;
        }

        public bool IsMatch(IEnumerable<ICombatCommand> sequence)
        {
         
            // Convert the sequence to an array to facilitate direct indexing.
            // Materialize only the part of the sequence needed for this rule.
            var sequenceArray = sequence.Take(ComboLength).ToArray();

            // Early exit if the sequence is shorter than the number of commands in the combo.
            if (sequenceArray.Length < ComboLength)
            {
                return false;
            }

            // Assigning to local variables for clarity and to improve readability.
            var first = sequenceArray[0];
            var second = sequenceArray[1];


            return first is BackInputCommand && second is BackInputCommand;
        }

        /// <summary>
        /// Produces the combo action command associated with the "Up+B" pattern.
        /// </summary>
        /// <returns>A new instance of a JumpKickActionCommand.</returns>
        public ICombatCommand GetResultingComboCommand()
        {
            return _comboActionCommandFactory.CreateNewDodgeBackCommand();
        }
    }

