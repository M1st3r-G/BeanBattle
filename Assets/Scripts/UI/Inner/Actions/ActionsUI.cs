using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inner.Actions
{
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
        
        public void SetDisplay(List<CharacterAction> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                _actionCells[i].gameObject.SetActive(true);
                _actionCells[i].SetToAction(actions[i], i);
            }

            for (int i = actions.Count; i < 7; i++)
            {
                _actionCells[i].gameObject.SetActive(false);
            }

            _group.padding.right = (7 - actions.Count) * 150;
        }

        public void SetCellAtIndex(int idx, bool state, out CharacterAction actionInCell)
        {
            actionInCell = _actionCells[idx].Action;
            _actionCells[idx].SetSelected(state);
        }

        public CharacterAction GetActionWithIndex(int index) => _actionCells[index].Action;
    }
}