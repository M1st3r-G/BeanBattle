using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Saves all Information regarding a Character
    /// </summary>
    [CreateAssetMenu(fileName = "Character")]
    public class CharData : ScriptableObject
    {
        public List<CharacterAction> Actions => action;
        [SerializeField] private List<CharacterAction> action = new();
        
        public Material Material(int teamId) => coloredMaterial[teamId];
        [SerializeField] private Material[] coloredMaterial;

        public Material Shadow(int teamId) => shadowMaterial[teamId];
        [SerializeField] private Material[] shadowMaterial;
        
        public string Name => charName;
        [SerializeField] private string charName;
        
        public int BaseHealth => startHealth;
        [SerializeField] private int startHealth;

        public Vector2Int InitiativeStartRange => iniRange;
        [SerializeField] private Vector2Int iniRange;

        public int AttackRange => attackRange;
        [SerializeField] private int attackRange;
        
        public int Damage => baseDamage;
        [SerializeField] private int baseDamage;
    }
}