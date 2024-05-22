using Data;
using Managers;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    [RequireComponent(typeof(CharStateController))]
    public class CharController : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private GameObject shadow;
        [SerializeField] private GameObject selector;
        private MeshRenderer _renderer;
        private CharStateController _stateController;
        public GameObject Indicator { get; private set; }
        
        // PubicTemps
        public int TeamID { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Initiative { get; set; }
        public CharData GetData { get; private set; }
        
        // Events
        public delegate void OnPlayerDeathEvent(CharController c);
        public static OnPlayerDeathEvent OnPlayerDeath;

        public delegate void OnPlayerFinishedActionEvent(CharacterAction.ActionTypes type);
        public static OnPlayerFinishedActionEvent OnPlayerFinishedAnimation;
        public delegate void OnPlayerStartedActionEvent(CharacterAction.ActionTypes type);
        public static OnPlayerStartedActionEvent OnPlayerStartedAnimation;
        
        #region Setup
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _stateController = GetComponent<CharStateController>();
            selector = transform.GetChild(0).gameObject;
        }

        public void Init(CharData characterClass, int team)
        {
            GetData = characterClass;
            TeamID = team;
            
            _renderer.material = GetData.Material(TeamID);
            Initiative = Random.Range(GetData.InitiativeStartRange.x, GetData.InitiativeStartRange.y);
            name = GetData.Name + $"(Team {TeamID})";
            CurrentHealth = GetData.BaseHealth;
            
            Indicator = CreateIndicator();
        }
        
        private GameObject CreateIndicator()
        {
            GameObject ind = Instantiate(shadow, transform.position, Quaternion.identity, transform);
            ind.GetComponent<MeshRenderer>().material = GetData.Shadow(TeamID); // Maybe Own Script if more Complex
            ind.name += "(shadow)";
            ind.SetActive(false);
            return ind;
        }
        #endregion

        #region Methods
        public void AddInitiative()
        {
            Initiative += UIManager.Instance.GetTimeCost();
            GameManager.Instance.RefreshInitiative(this);
        }
        
        public void PerformAttack(CharController other)
        {
            other.TakeDamage(GetData.Damage);
            UIManager.Instance.RefreshCharacter(other);
        }

        private void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            if (CurrentHealth > 0) return;
            
            OnPlayerDeath?.Invoke(this);
            Destroy(gameObject);
        }

        public void TriggerState(CharacterAction.ActionTypes type) => _stateController.SwitchState(type);
        public void EndState() => _stateController.SwitchState(CharacterAction.ActionTypes.None);
        public void SetSelector(bool state) => selector.SetActive(state);
        public override string ToString() => $"{name} ({CurrentHealth}): {Initiative}";
        #endregion
    }
}
