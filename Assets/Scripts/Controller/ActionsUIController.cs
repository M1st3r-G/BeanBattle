using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Controller
{
    public class ActionsUIController : MonoBehaviour
    {
        private ActionCell[] _actionCells = new ActionCell[7];

        private void Awake()
        {
            _actionCells = GetComponentsInChildren<ActionCell>(true);
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
        }
    }
}