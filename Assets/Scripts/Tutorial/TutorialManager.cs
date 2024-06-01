using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private InputActionReference mouseClick;

        [SerializeField] private TutorialUI display;

        //Params
        //Temps
        private CharController CurrentPlayer { get; set; }
        private List<CharController> _playOrder;

        private bool continueOrder;
        //Public

        public static TutorialManager Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
            continueOrder = false;
            SetUp();
        }

        private void SetUp()
        {
            _playOrder = GameObject.FindGameObjectsWithTag("Character").Select(gO => gO.GetComponent<CharController>())
                .ToList();
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
        }
        
        private void Start()
        {
            StartCoroutine(Tutorial());
        }

        private IEnumerator Tutorial()
        {
            UIManager.Instance.HideAll();
            mouseClick.action.Enable();

            display.Next(); //Willkommen
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());

            display.Next(); //Spieler
            UIManager.Instance.ShowInitiative();
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());



            UIManager.Instance.ShowCurrentCharacter(); // And actionsUI
            yield return NextPlayer();
            display.Next(); // Hier siehst du
            yield return new WaitUntil(() => continueOrder);

            UIManager.Instance.ShowActiveAction();
            CurrentPlayer.TriggerCharacterState(CharacterAction.ActionTypes.Move);
            display.Next(); // Bewegungsmodus

            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
        }

        public void TriggerState()
        {
            continueOrder = true;
        }

        private IEnumerator NextPlayer()
        {
            if (CurrentPlayer is not null)
            {
                CurrentPlayer.SkipState();
                //Add him to the Order and Reorders, then Displays the List
                _playOrder.Add(CurrentPlayer);
                _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
                UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
            }

            //Count Time Down (step by Step)
            int min = _playOrder.Min(c => c.Initiative);
            if (min > 0)
            {
                int counter = 0;
                while (counter < min)
                {
                    float length = AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Ticking);
                    yield return new WaitForSeconds(length);
                    foreach (CharController p in _playOrder) p.Initiative--;
                    UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
                    counter++;
                }

            }

            // Set and Remove the Next Player
            CurrentPlayer = _playOrder[0];
            _playOrder.RemoveAt(0);
        }
    }
}