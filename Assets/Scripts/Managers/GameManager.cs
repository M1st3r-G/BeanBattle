using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputActionReference nextPhaseAction;
        private bool _nextPhasePressed;
        
        private IEnumerator Start()
        {
            print("SetUp");
            yield return new WaitForSeconds(0.5f);
            
            while(true){
                print("PlayerPhase");
                _nextPhasePressed = false;
                nextPhaseAction.action.Enable();

                yield return new WaitUntil(() => _nextPhasePressed);
                nextPhaseAction.action.Disable();
            }
        }

        private void OnEnable()
        {
            nextPhaseAction.action.performed += ctx => _nextPhasePressed = true;
        }
    }
}