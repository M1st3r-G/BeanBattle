using System;
using System.Collections;
using Data;
using Managers;
using Unity.VisualScripting;
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

        public void TriggerState(CharacterAction.ActionTypes type)
        {
            print($"Ich, {this} muss jetzt {type} ausführen");
            switch (type)
            {
                case CharacterAction.ActionTypes.Move:
                    if(currentState is not null) StopCoroutine(currentState);
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

        private IEnumerator MoveState()
        {
            while (true)
            {
                Vector3Int? hoveredCell = MouseInputManager.Instance.GetCellFromMouse();
                if (hoveredCell is not null)
                {
                    Debug.Log((Vector2Int)hoveredCell);
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