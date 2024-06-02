using Data;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controller
{
    [RequireComponent(typeof(CharStateController))]
    public class CharController : MonoBehaviour
    {
        #region Fields

        //ComponentReferences
        [SerializeField] private GameObject shadow;
        [SerializeField] private GameObject selector;
        private MeshRenderer _renderer;
        private CharStateController _stateController;
        private AttackController _attackController;
        //PublicComponentReferences
        public GameObject Indicator { get; private set; }
        // PublicTemps
        public int TeamID { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Initiative { get; set; }
        public CharData GetData { get; private set; }
        
        // Events
        public delegate void OnPlayerDeathEvent(CharController c);
        public static OnPlayerDeathEvent OnPlayerDeath;

        #endregion
        
        #region Setup
        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _stateController = GetComponent<CharStateController>();
            _attackController = GetComponentInChildren<AttackController>();
            selector = transform.GetChild(0).gameObject;
        }

        /// <summary>
        /// Sets the Character Class and it's team after Object-generation
        /// </summary>
        /// <param name="characterClass">The Type of Character (Sets Stats)</param>
        /// <param name="team">The Team (Set's the Color and Team)</param>
        public void Init(CharData characterClass, int team)
        {
            GetData = characterClass;
            Initiative = Random.Range(GetData.InitiativeStartRange.x, GetData.InitiativeStartRange.y);
            CurrentHealth = GetData.BaseHealth;
            
            TeamID = team;
            name = GetData.Name + $"(Team {TeamID})";
            _renderer.material = GetData.Material(TeamID);
            
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
        
        public void AddInitiative(int amount)
        {
            //Add Initiative and Display
            Initiative += amount;
            UIManager.Instance.RefreshCharacter(this);
        }

        public void TakeDamage(int amount)
        {
            //Take Damage and Display
            CurrentHealth -= amount;
            UIManager.Instance.RefreshCharacter(this);
            
            //On Death Trigger Event and Destroy
            if (CurrentHealth > 0) AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Hit);
            else
            {
                OnPlayerDeath?.Invoke(this);
                Destroy(gameObject);
                Debug.Log($"{name} has Died");
                AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Death);
            }
        }
        
        public float Attack(Transform target) => _attackController.Attack(target);
        public void SkipState() => _stateController.SkipState();
        public void TriggerCharacterState(CharacterAction.ActionTypes type) => _stateController.TriggerState(type);
        public void SetSelector(bool state) => selector.SetActive(state);
        public override string ToString() => $"{name} ({CurrentHealth}): {Initiative}";
        #endregion
    }
}
