using System;
using System.Collections;
using Data;
using Managers;
using Misc;
using UI;
using UI.CurrentCharacter;
using UI.Initiative;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Controller
{
    public class CharController : MonoBehaviour
    {
        //Components
        [SerializeField] private InputActionReference stopAction;
        public CharData GetData => data;
        [SerializeField] private CharData data;
        private MeshRenderer _renderer;
        private Coroutine _currentState;
        private CharacterAction.ActionTypes _stateType;
        private GameObject _indicator;
        
        public int CurrentHealth { get; private set; }
        public int Initiative { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material = data.Material;

            Initiative = Random.Range(data.InitiativeStartRange.x, data.InitiativeStartRange.y);
            
            name = data.Name;
            CurrentHealth = data.BaseHealth;
            _currentState = null;
            _stateType = CharacterAction.ActionTypes.None;
        }

        private void Start()
        {
            _indicator = CreateIndicator();
        }

        private GameObject CreateIndicator()
        {
            GameObject ind = Instantiate(gameObject, transform.position, Quaternion.identity, transform);
            DestroyImmediate(ind.GetComponent<CharController>());
            ind.GetComponent<MeshRenderer>().material = data.Shadow;
            ind.name += "(shadow)";
            ind.SetActive(false);
            return ind;
        }

        public void TriggerState(CharacterAction.ActionTypes type)
        {
            if (_stateType == type)
            {
                EndState();
                return;
            }
            if (_currentState is not null) EndState();
            
            stopAction.action.Enable();
            _stateType = type;
            switch (type)
            {
                case CharacterAction.ActionTypes.Move:
                    _currentState = StartCoroutine(MoveState());
                    break;
                case CharacterAction.ActionTypes.Attack:
                    break;
                case CharacterAction.ActionTypes.BeEvil:
                    break;
                case CharacterAction.ActionTypes.Help:
                    break;
                case CharacterAction.ActionTypes.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "None kann nicht getriggert werden");
            }
        }

        public void EndState()
        {
            //General
            if(_currentState is not null) StopCoroutine(_currentState);
            _currentState = null;
            _stateType = CharacterAction.ActionTypes.None;
            stopAction.action.Disable();
            CurrentCharacterUIController.Instance.DeselectCurrentAction();
            
            //Personal
            _indicator.SetActive(false);
        }
        
        private IEnumerator MoveState()
        {
            _indicator.SetActive(true);
            bool stateIsActive = true;
            int timeCost = 0;
            
            while (stateIsActive)
            {
                if(MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell))
                {
                    if (!GridManager.Instance.IsOccupied(hoveredCell))
                    {
                        Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                        newPosition.y = transform.position.y;
                        _indicator.transform.position = newPosition;
                        
                        timeCost = GridManager.Instance.GetPosition(this).ManhattanDistance(hoveredCell);
                        CurrentActionController.Instance.SetTimeCost(timeCost);
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        print("Accepted");
                        transform.position = _indicator.transform.position;
                        GridManager.Instance.SetOccupied(this, hoveredCell);
                        stateIsActive = false;
                    }
                }

                if (stopAction.action.WasPerformedThisFrame()) stateIsActive = false;
                
                yield return null;
            }

            AddInitiative(timeCost);
            EndState();
        }

        private void AddInitiative(int val)
        {
            Initiative += val;
            GameManager.Instance.RefreshInitiative(this);
        }
        
        public override string ToString()
        {
            return $"{name} ({CurrentHealth}): {Initiative}";
        }
        
    }
}