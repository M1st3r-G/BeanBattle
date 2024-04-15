using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Character")]
    public class CharData : ScriptableObject
    {
        public List<CharacterActionPacket> Actions => action;
        [SerializeField] private List<CharacterActionPacket> action = new();
        
        public Material Material => coloredMaterial;
        [SerializeField] private Material coloredMaterial;

        public string Name => charName;
        [SerializeField] private string charName;
        
        public int BaseHealth => startHealth;
        [SerializeField] private int startHealth;

        public Vector2Int InitiativeStartRange => iniRange;
        [SerializeField] private Vector2Int iniRange;
    }
}