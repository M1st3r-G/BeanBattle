using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Library", menuName = "States/Library", order = 0)]
    public class StateLibrary : ScriptableObject
    {
        public CharacterStateBase EmptyState => emptyState;
        [SerializeField] private CharacterStateBase emptyState;
        
        [SerializeField] private CharacterStateBase[] states;

        public CharacterStateBase GetState(CharacterAction.ActionTypes type)
        {
            CharacterStateBase[] res = states.Where(s => s.Type == type).ToArray();
            Debug.Assert(res.Length == 1, $"Fehler! Es wurden {res.Length} Möglichen States gefunden");
            return res[0];
        }
    }
}