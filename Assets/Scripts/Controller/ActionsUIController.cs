using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Controller
{
    public class ActionsUIController : MonoBehaviour
    {
        private ActionCellController[] _actionCells = new ActionCellController[7];
        private HorizontalLayoutGroup _group;
        
        private void Awake()
        {
            _actionCells = GetComponentsInChildren<ActionCellController>(true);
            _group = GetComponent<HorizontalLayoutGroup>();
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

            FixBorder(actions.Count);
        }

        private void FixBorder(int numberOfActives)
        {
            _group.padding.right = (7 - numberOfActives) * 150;
        }
    }
}