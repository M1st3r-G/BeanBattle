using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CurrentCharacter
{
    public class ActionsUIController : MonoBehaviour
    {
        // ComponentReferences
        private ActionCellController[] _actionCells = new ActionCellController[7];
        private HorizontalLayoutGroup _group;

        // Temps
        public int CurrentSelection { get; private set; } = -1;

        #region SetUp
        private void Awake()
        {
            _actionCells = GetComponentsInChildren<ActionCellController>(true);
            _group = GetComponent<HorizontalLayoutGroup>();
        }
        #endregion

        #region MainMethods
        public void Select(int idx)
        {
            if(CurrentSelection != -1) _actionCells[CurrentSelection].SetSelected(false);
            CurrentSelection = idx;
            _actionCells[CurrentSelection].SetSelected(true);
        }

        public void Deselect()
        {
            if (CurrentSelection == -1) return;
            
            _actionCells[CurrentSelection].SetSelected(false);
            CurrentSelection = -1;
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

            _group.padding.right = (7 - actions.Count) * 150;
        }
        #endregion
    }
}