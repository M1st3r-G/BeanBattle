using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CurrentCharacter
{
    public class ActionsUIController : MonoBehaviour
    {
        private ActionCellController[] _actionCells = new ActionCellController[7];
        private HorizontalLayoutGroup _group;

        private int currentSelection = -1;
        
        private void Awake()
        {
            _actionCells = GetComponentsInChildren<ActionCellController>(true);
            _group = GetComponent<HorizontalLayoutGroup>();
        }

        public void Select(int idx)
        {
            if(currentSelection != -1) _actionCells[currentSelection].SetSelected(false);
            currentSelection = idx;
            _actionCells[currentSelection].SetSelected(true);
        }
        
        public void SetDisplay(List<CharacterAction> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                _actionCells[i].gameObject.SetActive(true);
                _actionCells[i].SetTo(actions[i], i + 1);
            }

            for (int i = actions.Count; i < 7; i++)
            {
                _actionCells[i].gameObject.SetActive(false);
            }

            FixLayout(actions.Count);
        }

        private void FixLayout(int numberOfActives)
        {
            _group.padding.right = (7 - numberOfActives) * 150;
        }
    }
}