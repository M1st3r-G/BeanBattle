using System.Collections.Generic;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UIContent.Actions
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
        public void SetCellStateAtIndex(int idx, bool state)
            => _actionCells[idx].SetSelectionState(state);

        public CharacterAction GetActionWithIndex(int index) => _actionCells[index].Action;
        
        /// <summary>
        /// Returns the Index of the Cell with the given type
        /// Return -1 if not found
        /// </summary>
        /// <param name="type">The type to search for</param>
        /// <param name="action">The ActionAssest</param>
        /// <returns>The Index of the Cell (zero Based)</returns>
        public int GetIndexWithType(CharacterAction.ActionTypes type, out CharacterAction action)
        {
            for (var index = 0; index < _actionCells.Length; index++)
            { 
                action = _actionCells[index].Action;
                if (action.Type == type) return index;
            }

            action = null;
            return -1;
        }
    }
}