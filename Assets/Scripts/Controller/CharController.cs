using Data;
using UnityEngine;

namespace Controller
{
    public class CharController : MonoBehaviour
    {
        //Components
        public CharData GetData => data;
        [SerializeField] private CharData data;
        private MeshRenderer _renderer;

        public int CurrentHealth { get; private set; }

        public int Initiative { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material = data.Material;

            Initiative = Random.Range(data.InitiativeStartRange.x, data.InitiativeStartRange.y);
            
            name = data.Name;
            CurrentHealth = data.BaseHealth;
        }

        public void TriggerState(CharacterAction.ActionTypes type)
        {
            print($"Ich, {this} muss jetzt {type} ausführen");
        }
        
        public override string ToString()
        {
            return $"{name} ({CurrentHealth}): {Initiative}";
        }
        
    }
}