using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Controller;
using Data;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        //ComponentReferences
        [SerializeField] private InputActionReference mouseClick;
        [SerializeField] private TutorialUI display;
        [SerializeField] private GameObject target;
        [SerializeField] private CharController[] players;
        [SerializeField] private CharData[] classes;
        
        //Params
        //Temps
        private CharController CurrentPlayer { get; set; }
        private List<CharController> _playOrder;

        private bool _continueOrder;

        private Vector2Int[] _path;
        //Public

        public static TutorialManager Instance { get; private set; }


        private void Awake()
        {
            Instance = this;
            _continueOrder = false;
            target.SetActive(false);
        }

        private void SetUp()
        {
            for (int i = 0; i < 2; i++)
            {
                players[i].Init(classes[i], i);
                // Notify the Grid to set the StartCell Occupied
                GridManager.Instance.AddCharacter(players[i], GridManager.Instance.WorldToCell(players[i].transform.position));
            }

            _playOrder = players.ToList();
            _playOrder.Sort((l, r) => l.Initiative.CompareTo(r.Initiative));
        }
        
        private void Start()
        {            
            SetUp();
            StartCoroutine(Tutorial());
        }

        private IEnumerator Tutorial()
        {
            UIManager.Instance.HideAll();
            mouseClick.action.Enable();

            display.Next(); //Willkommen
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
            yield return null;
            
            display.Next(); //Spieler
            UIManager.Instance.ShowInitiative();
            UIManager.Instance.ChangeInitiativeOrderTo(_playOrder.ToArray());
            yield return NextPlayer();
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
            yield return null;

            UIManager.Instance.ShowCurrentCharacter(); // And actionsUI
            UIManager.Instance.ChangeActiveCharacter(CurrentPlayer);
            CustomInputManager.Instance.EnableInput(); // Enables number Shortcuts
            display.Next(); // Hier siehst du
            yield return new WaitUntil(() => _continueOrder);
            _continueOrder = false;
            
            UIManager.Instance.ShowActiveAction();
            UIManager.Instance.DisplayNewAction(CharacterAction.ActionTypes.Move);
            display.Next(); // Bewegungsmodus
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
            yield return null;
            
            display.Next(); // Während Bewegungsmodus
            target.SetActive(true);
            yield return null;
            yield return Movement();
            
            display.Next(); // Du hast verstanden, wie man sich Bewegt
            UIManager.Instance.DeselectCurrentAction();
            CurrentPlayer.AddInitiative(UIManager.Instance.GetTimeCost());
            CustomInputManager.Instance.EnableInput();
            yield return new WaitUntil(() => _continueOrder);
            _continueOrder = false;

            yield return NextPlayer();
            UIManager.Instance.ChangeActiveCharacter(CurrentPlayer);
            CustomInputManager.Instance.EnableInput(); // Enables number Shortcuts
            display.Next(); // Dieser Zweite Character
            yield return new WaitUntil(() => _continueOrder);
            _continueOrder = false;
            
            display.Next(); // Angriffsmodus
            yield return Attack();
            
            display.Next(); // Sehr gut
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
            yield return null;
            
            display.Next(); // Ende
            yield return new WaitUntil(() => mouseClick.action.WasPerformedThisFrame());
            SceneManager.LoadScene(0);
        }

        public void Continue() => _continueOrder = true;

        private IEnumerator NextPlayer()
        {
            if (CurrentPlayer is not null)
            {
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

        #region States
        
        private IEnumerator Movement()
        {
            //SetUp
            CurrentPlayer.Indicator.SetActive(true);
            _path = null;
            
            // Play Audio
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Move);

            //Loop
            bool running = true;
            while (running)
            {
                // If(Input) ChangeState
                if (!MouseInputManager.Instance.GetCellFromMouse(out Vector2Int hoveredCell))
                {
                    yield return null;
                }
                if (!GridManager.Instance.IsOccupied(hoveredCell))
                {
                    Debug.LogError($"Position:  {hoveredCell}");
                    // Stop showing Enemy's Ranges
                    GridManager.Instance.ResetRange();
                
                    // Display new Position with the Indicator
                    Vector3 newPosition = GridManager.Instance.CellToCenterWorld(hoveredCell);
                    newPosition.y = CurrentPlayer.transform.position.y;
                    CurrentPlayer.Indicator.transform.position = newPosition;
                
                    // Calculates and Renders Path
                    Vector2Int startCell = GridManager.Instance.WorldToCell(CurrentPlayer.transform.position);
                    _path = GridManager.Instance.GetPath(startCell, hoveredCell);
                    GridManager.Instance.RenderPath(_path);
                
                    // Adjusts the Action Cost
                    UIManager.Instance.SetTimeCost(_path.Length);
                }
                else
                {
                    // When hovering over Enemy (cells) it its range is displayed
                    GridManager.Instance.ResetRange();
                    GridManager.Instance.DisplayRange(GridManager.Instance.GetOccupier(hoveredCell));
                }

                // If(Accept)
                if (CustomInputManager.Instance.MouseClickedThisFrame() &&
                    hoveredCell == GridManager.Instance.WorldToCell(target.transform.position))
                {
                    Debug.Log("Movement Accepted!");
                    running = false;
                }

                yield return null;
            }
            
            Debug.Log("Finished the MovementInput");
            
            // Animate
            CurrentPlayer.Indicator.SetActive(false);
            target.SetActive(false);
            CustomInputManager.Instance.DisableInput();
            // for each field in the Path
            foreach (Vector2Int currentTargetCell in _path)
            {
                // Lerp position
                Vector3 startPosition = CurrentPlayer.transform.position;
                Vector3 endPosition = GridManager.Instance.CellToCenterWorld(currentTargetCell);

                float t = 0f;
                float timeForSpace = AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Steps);
                
                while (t<timeForSpace)
                {
                    CurrentPlayer.transform.position = Vector3.Lerp(startPosition, endPosition, t / timeForSpace);
                    t += Time.deltaTime;
                    yield return null;
                }

                GridManager.Instance.HideNextPathCell();
            }
            
            GridManager.Instance.SetOccupied(CurrentPlayer, _path[^1]);
            
            CurrentPlayer.Indicator.SetActive(false);
        }

        private IEnumerator Attack()
        {
            //SetUp
            AudioEffectsManager.Instance.PlayEffect(AudioEffectsManager.AudioEffect.Attack);
            GridManager.Instance.ResetRange();
            GridManager.Instance.DisplayRange(CurrentPlayer);
            
            _playOrder[^1].SetSelector(true);

            yield return new WaitUntil(() => CustomInputManager.Instance.AcceptedThisFrame());

            // Animate
            _playOrder[^1].TakeDamage(CurrentPlayer.GetData.Damage);
        }
        
        #endregion
    }
}
