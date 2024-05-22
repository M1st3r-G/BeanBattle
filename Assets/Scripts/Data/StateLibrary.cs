using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Library", menuName = "States/Library", order = 0)]
    public class StateLibrary : ScriptableObject
    {
        // Component References
        public CharacterStateBase EmptyState => emptyState;
        [SerializeField] private CharacterStateBase emptyState;
        [SerializeField] private CharacterStateBase[] states;

        public CharacterStateBase GetState(CharacterAction.ActionTypes type) =>
            states.Where(s => s.ActionType == type).ToArray()[0];
    }
}