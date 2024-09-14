using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxUI : MonoBehaviour
{

    [SerializeField] PointerSelector pointer;
    [SerializeField] BoxInfoOverlay boxInfoOverlay;
    [SerializeField] GameObject partyBoxOverlay;
    [SerializeField] GameObject buttons;
    [SerializeField] BoxHeaderUI boxHeaderUI;

    [SerializeField] List<BoxPokemon> boxSlots;
    [SerializeField] List<BoxPokemon> partySlots;

    const int BOX_LIMIT = 36;
    const int ROW_LIMIT = 6;

    const int PARTY_BUTTON = BOX_LIMIT;
    const int EXIT_BUTTON = 37;
    const int FIRST_PARTY_MEMBER = 38;
    const int SELECTION_LIMIT = 43;
    const int CLOSE_PARTY_BUTTON = 44;
    const int BANNER_SELECTION = -1;
    const int N_BOXES = 10;
    readonly List<int> buttonsList = new List<int>() {BANNER_SELECTION, PARTY_BUTTON, EXIT_BUTTON, CLOSE_PARTY_BUTTON};

    int boxSelection = 0;
    int selectionIndex = 0;
     
    // Button 0 = party, button 1 = exit
    int buttonSelection = 0;
    int partySelection = 0;

    // Given swapping requires two selections, have we picked anything up?
    bool holdingPokemon = false;
    int heldPokemonIndex = -1;
    int heldPokemonBox = -1;


    PokemonParty party;
    PokemonStorage pokemonBoxes;

    private void Start()
    {
        party = PokemonParty.GetPlayerParty();
        pokemonBoxes = PokemonStorage.GetPlayerStorageBoxes();

        Debug.Log("length of partySlots is " + partySlots.Count);
        Debug.Log("length of party is " + party.PokemonList.Count);
        boxSelection = 0;
        List<Pokemon> box = pokemonBoxes.GetBoxByIndex(boxSelection);
        boxHeaderUI.SetBoxNumber(boxSelection);
        for (int i = 0; i < boxSlots.Count; i++)
        {
            boxSlots[i].SetData(box[i]);
        }

        for (int i = 0; i < party.PokemonList.Count; i++)
            partySlots[i].SetData(party.PokemonList[i]);
        for (int i = party.PokemonList.Count; i < partySlots.Count; i++)
            partySlots[i].SetData(null);

        UpdateHoverSelection(0, 0);
    }

    public void HandleUpdate()
    {
        if (Input.GetButtonDown("Selection"))
        {
            if (selectionIndex == PARTY_BUTTON)
            {
                // Open party, move cursor, place shadow and cursor on party member 0
                
                partyBoxOverlay.SetActive(true);
                UpdateHoverSelection(selectionIndex, FIRST_PARTY_MEMBER);
                selectionIndex = FIRST_PARTY_MEMBER;
            }
            else if (selectionIndex == EXIT_BUTTON)
            {
                if (holdingPokemon)
                {
                    // Trying to exit box while holding pokemon. Show "You can't do that while holding a pokemon!" dialogue and noise
                }
                else 
                {
                    // Open do you want to exit dialogue choice
                }
            }
            else if (selectionIndex > BANNER_SELECTION && selectionIndex <= 43)
            {
                // If a valid space is selected
                if (0 <= selectionIndex && selectionIndex < BOX_LIMIT)
                {
                    // In box
                    if (!holdingPokemon)
                    {
                        if (pokemonBoxes.GetPokemon(boxSelection, selectionIndex)!= null)
                        {
                            // Open choice dialogue
                        }
                    }
                    else
                    {
                        if (pokemonBoxes.GetPokemon(boxSelection,selectionIndex) != null)
                        {
                            // open choice dialogue
                        }
                        else
                        {
                            // just place and continue
                        }
                    }
                }

            }
            else if (selectionIndex == 44)
            {
                // party close button
                selectionIndex = 0;
                partyBoxOverlay.SetActive(false);
                UpdateHoverSelection(44, selectionIndex);
            }
 
        }
        else if (Input.GetButtonDown("GoBack"))
        {
            if (selectionIndex > EXIT_BUTTON)
            {
                // In party, so exit party
                selectionIndex = 0;
                // close overlay
            }
            else if (!holdingPokemon)
            {
                // open choice box to leave party 

            }
            else
            {
                // Cannot leave box with pokemon in hand, must put down, tell player
            }
        }
        else
        {
            int prev = selectionIndex;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (30 <= selectionIndex && selectionIndex < 34)
                    selectionIndex = PARTY_BUTTON;
                else if (selectionIndex == 34 || selectionIndex == 35)
                    selectionIndex = EXIT_BUTTON;
                else if (prev < BOX_LIMIT && prev > -1)
                    selectionIndex += ROW_LIMIT;
                else if (prev > EXIT_BUTTON)
                    selectionIndex += 2;
                else if (prev == EXIT_BUTTON || prev == PARTY_BUTTON)
                    selectionIndex = -1;
                else if (prev == BANNER_SELECTION)
                    selectionIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (prev == EXIT_BUTTON || prev == PARTY_BUTTON)
                    selectionIndex = BOX_LIMIT - 6;
                else if (prev == BANNER_SELECTION)
                    selectionIndex = PARTY_BUTTON;
                else if (prev < BOX_LIMIT)
                    selectionIndex -= ROW_LIMIT;
                else if (prev > EXIT_BUTTON)
                    selectionIndex -= 2;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (prev == -1)
                {
                    // reload box with new pokemon
                    boxSelection--;
                    if (boxSelection == -1)
                        boxSelection = N_BOXES - 1;
                    SwitchBox();
                }
                else if (prev == PARTY_BUTTON)
                    selectionIndex = EXIT_BUTTON;
                else if (prev == EXIT_BUTTON)
                    selectionIndex = PARTY_BUTTON;
                else if (prev < BOX_LIMIT || (prev > EXIT_BUTTON && (prev % 2 == 1)))
                    selectionIndex--;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (prev == -1)
                {
                    // reload box with new pokemon
                    boxSelection++;
                    if (boxSelection >= N_BOXES)
                        boxSelection = 0;
                    SwitchBox();
                    
                }
                else if (prev == PARTY_BUTTON)
                    selectionIndex = EXIT_BUTTON;
                else if (prev == EXIT_BUTTON)
                    selectionIndex = PARTY_BUTTON;
                else if (prev > EXIT_BUTTON && (prev % 2 == 1))
                {
                    selectionIndex = 0;
                    partyBoxOverlay.SetActive(false);
                    UpdateHoverSelection(prev, selectionIndex);
                    // remove overlay, go back to current box with selection = 0
                }
                else
                    selectionIndex++;
            }

            selectionIndex = Mathf.Clamp(selectionIndex, -1, 44);
            if (selectionIndex != prev)
            {
                UpdateHoverSelection(prev, selectionIndex);
            }

        }
    }

    

    public void BoxReset()
    {
        boxSelection = 0;
        buttonSelection = 0;
        partySelection = BOX_LIMIT;
        holdingPokemon = false;
        heldPokemonIndex = -1;
    }


    public void UpdateHoverSelection(int prevSelection, int currentSelection)
    {
        Debug.Log("Updating cursor with new selection = " + currentSelection + " and prev = " + prevSelection);
        pointer.Move(GetLocation(currentSelection));

        // If moving from a pokemon, deselect it
        if (!buttonsList.Contains(prevSelection))
        {
            if (prevSelection < BOX_LIMIT)
                boxSlots[prevSelection].HoverDeselect();
            else if (prevSelection > EXIT_BUTTON)
                partySlots[prevSelection - FIRST_PARTY_MEMBER].HoverDeselect();
        }

        // If on a pokemon 
        if (!buttonsList.Contains(currentSelection) )
        {
            // If box pokemon
            if (currentSelection < BOX_LIMIT)
            {
                var current = boxSlots[currentSelection];
                current.HoverSelect();
                if (!holdingPokemon)
                    boxInfoOverlay.ShowDetails(pokemonBoxes.GetPokemon(boxSelection, currentSelection));
            }
            else
            {
                var current = partySlots[currentSelection - FIRST_PARTY_MEMBER];
                current.HoverSelect();
                if (!holdingPokemon)
                    boxInfoOverlay.ShowDetails(current.GetPokemon());
            }
            
        }
            
    }

    void UpdatePickupSelection()
    {

    }

    void UpdateDropSelection()
    {

    }

    void SwitchBox()
    {
        List<Pokemon> box = pokemonBoxes.GetBoxByIndex(boxSelection);
        boxHeaderUI.SetBoxNumber(boxSelection);
        for (int i = 0; i < boxSlots.Count; i++)
        {
            boxSlots[i].SetData(box[i]);
        }

    }

    Transform GetLocation(int selection)
    {
        if (selection == -1)
        {
            return boxHeaderUI.transform;
        }
        if (selection < BOX_LIMIT)
        {
            return boxSlots[selection].transform;
        }
        if (selection == BOX_LIMIT)
            return buttons.transform.GetChild(0).transform;
        if (selection == EXIT_BUTTON)
            return buttons.transform.GetChild(1).transform;
        if (selection < CLOSE_PARTY_BUTTON)
            return partySlots[selection - FIRST_PARTY_MEMBER].transform;

        return partyBoxOverlay.transform.GetChild(6).transform;
            
    }


}
