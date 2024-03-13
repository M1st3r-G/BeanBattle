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

        public int CurrentHealth => _health;
        private int _health;

        public int Initiative
        {
            get => _initiative;
            set => _initiative = value;
        }

        private int _initiative;
        
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material = data.Material;

            _initiative = Random.Range(data.InitiativeStartRange.x, data.InitiativeStartRange.y);
            
            name = data.Name;
            _health = data.BaseHealth;
        }

        public override string ToString()
        {
            return $"{name} ({_health}): {_initiative}";
        }
        
    }
}