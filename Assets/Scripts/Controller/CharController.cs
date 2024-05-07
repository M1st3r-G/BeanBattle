using System;
using System.Collections;
using Data;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Controller
{
    public class CharController : MonoBehaviour
    {
        //Components
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
            if(_currentState is not null) StopCoroutine(_currentState);
            _currentState = null;
            _indicator.SetActive(false);
        }
        
        private IEnumerator MoveState()
        {
            _indicator.SetActive(true);
            while (true)
            {
                Vector3Int? hoveredCell = MouseInputManager.Instance.GetCellFromMouse();
                
                if (hoveredCell is not null)
                {
                    if (!GridManager.Instance.IsOccupied((Vector3Int)hoveredCell))
                    {
                        Vector3 newPosition = GridManager.Instance.CellToCenterWorld((Vector3Int)hoveredCell);
                        newPosition.y = transform.position.y;
                        _indicator.transform.position = newPosition;
                        GridManager.Instance.SetOccupied(this, (Vector3Int)hoveredCell);
                        CurrentActionController.Instance.SetTimeCost((int)Vector2.Distance(new Vector2(transform.position.x, transform.position.z), (Vector3)(Vector3Int)hoveredCell));
                    }

                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        print("Accepted");
                    }
                }

                yield return null;
            }
        }
        
        public override string ToString()
        {
            return $"{name} ({CurrentHealth}): {Initiative}";
        }
        
    }
}