using Data;
using Managers;
using UI;
using UI.Initiative;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    [RequireComponent(typeof(CharStateController))]
    public class CharController : MonoBehaviour
    {
        public CharData GetData => data;
        [SerializeField] private CharData data;
        
        [SerializeField] private GameObject shadow;
        [SerializeField] private GameObject selector;
        private MeshRenderer _renderer;
        private CharStateController _stateController;
        
        public GameObject Indicator { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Initiative { get; set; }

        public delegate void OnPlayerDeathEvent(CharController c);
        public static OnPlayerDeathEvent OnPlayerDeath;
        
        #region Setup
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _renderer.material = data.Material;

            Initiative = Random.Range(data.InitiativeStartRange.x, data.InitiativeStartRange.y);
            
            name = data.Name;
            CurrentHealth = data.BaseHealth;
            _stateController = GetComponent<CharStateController>();
            
            Indicator = CreateIndicator();
            selector = transform.GetChild(0).gameObject;
        }

        private GameObject CreateIndicator()
        {
            GameObject ind = Instantiate(shadow, transform.position, Quaternion.identity, transform);
            ind.GetComponent<MeshRenderer>().material = data.Shadow; // Maybe Own Script if more Complex
            ind.name += "(shadow)";
            ind.SetActive(false);
            return ind;
        }
        #endregion

        #region Methods

        public void AddInitiative()
        {
            Initiative += CurrentActionController.Instance.GetTimeCost();
            GameManager.Instance.RefreshInitiative(this);
        }
        
        public override string ToString()
        {
            return $"{name} ({CurrentHealth}): {Initiative}";
        }

        public void TriggerState(CharacterAction.ActionTypes type)
        {
            _stateController.SwitchState(type);
        }

        public void EndState()
        {
            _stateController.SwitchState(CharacterAction.ActionTypes.None);
        }

        public void SetSelector(bool state)
        {
            selector.SetActive(state);
        }
        
        public void PerformAttack(CharController other)
        {
            other.TakeDamage(data.Damage);
            InitiativeUIController.Instance.RefreshCharacter(other);
        }

        private void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth > 0) return;
            
            Debug.LogWarning($"{name} died!");
            OnPlayerDeath?.Invoke(this);
            Destroy(gameObject);
        }
        #endregion
    }
}