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
        public CharData GetData => data;
        [SerializeField] private CharData data;
        
        [SerializeField] private GameObject shadow;
        private MeshRenderer _renderer;
        private CharStateController _stateController;
        
        public GameObject Indicator { get; private set; }
        public int CurrentHealth { get; private set; }
        public int Initiative { get; set; }
        
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
        #endregion
    }
}