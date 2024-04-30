using System;
using System.Collections;
using Data;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    public class CharController : MonoBehaviour
    {
        //Components
        public CharData GetData => data;
        [SerializeField] private CharData data;
        private MeshRenderer _renderer;
        private Coroutine currentState ;
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
            currentState = null;
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
            print($"Ich, {this} muss jetzt {type} ausführen");
            if (currentState is not null) EndState();
            switch (type)
            {
                case CharacterAction.ActionTypes.Move:
                    currentState = StartCoroutine(MoveState());
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
            if(currentState is not null) StopCoroutine(currentState);
            currentState = null;
            _indicator.SetActive(false);
        }
        
        private IEnumerator MoveState()
        {
            _indicator.SetActive(true);
            while (true)
            {
                Vector3Int? hoveredCell = MouseInputManager.Instance.GetCellFromMouse();
                if (hoveredCell is null) yield return null;
                else
                {
                    if (MouseInputManager.Instance.IsOccupied((Vector3Int)hoveredCell)) yield return null;
                    else
                    {
                        Vector3 newPosition = MouseInputManager.Instance.CellToCenterWorld((Vector3Int)hoveredCell);
                        newPosition.y = transform.position.y;
                        _indicator.transform.position = newPosition;
                        MouseInputManager.Instance.SetOccupied(this, (Vector3Int)hoveredCell);
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