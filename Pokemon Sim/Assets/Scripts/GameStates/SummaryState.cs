using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class SummaryState : State<GameController>
{
    [SerializeField] SummaryScreenUI summaryScreen;

    // Input 
    public int PokemonIndex { get; set; }
    public int SelectedScreenIndex { get; private set; } = 0;

    const int nScreens = 3; 
    public static SummaryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        playerParty = PlayerController.i.GetComponent<PokemonParty>().PokemonList;
    }

    List<Pokemon> playerParty;
    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;
        summaryScreen.gameObject.SetActive(true);
        summaryScreen.SetData(playerParty[PokemonIndex]);
        summaryScreen.ShowPage(SelectedScreenIndex);
        summaryScreen.MoveScreen.ClearDescriptorData();
    }

    int selectedMove = 0;
    public override void Execute()
    {
        if (summaryScreen.InMoveSelection)
        {
            var prevSelectedMove = selectedMove;
            if (Input.GetButtonDown("GoBack"))
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                summaryScreen.MoveScreen.Clear();
                summaryScreen.InMoveSelection = false;
                selectedMove = 0;
                return;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                selectedMove++;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedMove--;

            selectedMove = Mathf.Clamp(selectedMove, 0, playerParty[PokemonIndex].Moves.Count - 1);
            if (prevSelectedMove != selectedMove)
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                summaryScreen.MoveScreen.SetMoveSelected(selectedMove);
                summaryScreen.MoveScreen.SetDescriptorData(playerParty[PokemonIndex].Moves[selectedMove]);

            }

        }
        else
        {
            if (Input.GetButtonDown("GoBack"))
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                gC.StateMachine.Pop();
                return;
            }
            else if (Input.GetButtonDown("Selection"))
            {
                AudioManager.i.PlaySFX(AudioID.UISelect);
                if (SelectedScreenIndex == 2 && !summaryScreen.InMoveSelection)
                {
                    selectedMove = 0;
                    summaryScreen.MoveScreen.SetMoveSelected(selectedMove);
                    summaryScreen.MoveScreen.SetDescriptorData(playerParty[PokemonIndex].Moves[selectedMove]);
                    summaryScreen.InMoveSelection = true;
                    return;
                }
                    
            }

            var prevPoke = PokemonIndex;
            var prevScreen = SelectedScreenIndex;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PokemonIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PokemonIndex--;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                SelectedScreenIndex--;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                SelectedScreenIndex++;

            if (prevPoke != PokemonIndex)
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                //Update on change
                if (PokemonIndex >= playerParty.Count)
                {
                    PokemonIndex = 0;
                }
                else if (PokemonIndex < 0)
                    PokemonIndex = playerParty.Count - 1;

                summaryScreen.SetData(playerParty[PokemonIndex]);
                summaryScreen.ShowPage(SelectedScreenIndex);
            }
            else if (prevScreen != SelectedScreenIndex)
            {
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
                if (SelectedScreenIndex >= nScreens)
                    SelectedScreenIndex = 0;
                else if (SelectedScreenIndex < 0)
                    SelectedScreenIndex = nScreens - 1;

                summaryScreen.ShowPage(SelectedScreenIndex);

            }
        }

            
        
    }

    public override void ExitState()
    {
        summaryScreen.gameObject.SetActive(false);
    }
}
