using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Actions
{
    /// <summary>
    /// Internally used by the <see cref="UIManager"/>. Uses <see cref="ActionCell"/> internally to Display and Manage Actions
    /// </summary>
    public class ActionsUI : MonoBehaviour
    {
        // ComponentReferences
        private ActionCell[] _actionCells = new ActionCell[7];
        private HorizontalLayoutGroup _group;

        private void Awake()
        {
            _actionCells = GetComponentsInChildren<ActionCell>(true);
            _group = GetComponent<HorizontalLayoutGroup>();
        }
        
        /// <summary>
        /// Displays a Given List of Actions
        /// </summary>
        /// <param name="actions">The Action to display (and their Order)</param>
        public void SetDisplay(List<CharacterAction> actions)
        {
            // Set the Action active and Set the Informations
            for (int i = 0; i < actions.Count; i++)
            {
                _actionCells[i].gameObject.SetActive(true);
                _actionCells[i].SetToAction(actions[i], i);
            }

            // Hide the Others
            for (int i = actions.Count; i < 7; i++)
            {
                _actionCells[i].gameObject.SetActive(false);
            }

            // Resets Padding
            _group.padding.right = (7 - actions.Count) * 150;
        }

        /// <summary>
        /// Set the Cell at the Index to the State. Also Returns the Action which the Cell Manages
        /// </summary>
        /// <param name="idx">The Index (Zero Based) of the Cell</param>
        /// <param name="state">Set the Cell Active (True) or Inactive (False)</param>
        /// <param name="actionInCell">The <see cref="CharacterAction"/> the Cell is Managing</param>
        public void SetCellStateAtIndex(int idx, bool state, out CharacterAction actionInCell)
        {
            actionInCell = _actionCells[idx].Action;
            _actionCells[idx].SetSelectionState(state);
        }

        /// <summary>
        /// Returns the Action at the given Index
        /// </summary>
        /// <param name="index">The (Zero Based) Index</param>
        /// <returns>The <see cref="CharacterAction"/> of the Cell at the Index</returns>
        public CharacterAction GetActionWithIndex(int index) => _actionCells[index].Action;
    }
}