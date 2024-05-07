using System;
using System.Collections;
using Data;
using Managers;
using Misc;
using UI;
using UI.CurrentCharacter;
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
        private Coroutine _currentState ;
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
            if (_currentState is not null) EndState();
            
            stopAction.action.Enable();
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void EndState()
        {
            //General
            if(_currentState is not null) StopCoroutine(_currentState);
            _currentState = null;
            stopAction.action.Disable();
            CurrentCharacterUIController.Instance.DeselectCurrentAction();
            
            //Personal
            _indicator.SetActive(false);
        }
        
        private IEnumerator MoveState()
        {
            _indicator.SetActive(true);
            bool stateIsActive = true;
            
            while (stateIsActive)
            {
                if(MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell))
                {
                    if (!GridManager.Instance.IsOccupied(hoveredCell))
                    {
                        Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                        newPosition.y = transform.position.y;
                        _indicator.transform.position = newPosition;
                        
                        int timeCost = GridManager.Instance.GetPosition(this).ManhattanDistance(hoveredCell);
                        CurrentActionController.Instance.SetTimeCost(timeCost);
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        print("Accepted");
                        transform.position = _indicator.transform.position;
                        GridManager.Instance.SetOccupied(this, hoveredCell);
                        stateIsActive = false;
                    }

                    if (stopAction.action.WasPerformedThisFrame())
                    {
                        stateIsActive = false;
                    }
                }

                yield return null;
            }
            
            EndState();
        }
        
        public override string ToString()
        {
            return $"{name} ({CurrentHealth}): {Initiative}";
        }
        
    }
}