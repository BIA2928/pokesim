using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxUI : MonoBehaviour
{
    const int BOX_LIMIT = 36;
    const int ROW_LIMIT = 6;
    const int PARTY_BUTTON = BOX_LIMIT;
    const int EXIT_BUTTON = 37;
    const int FIRST_PARTY_MEMBER = 38;
    const int SELECTION_LIMIT = 43;
    const int CLOSE_PARTY_BUTTON = 44;
    const int BANNER_SELECTION = -1;
    const int N_BOXES = 10;
    readonly List<int> buttonsList = new List<int>() { BANNER_SELECTION, PARTY_BUTTON, EXIT_BUTTON, CLOSE_PARTY_BUTTON };

    // Gameobjects
    [SerializeField] PointerSelector pointer;
    [SerializeField] BoxInfoOverlay boxInfoOverlay;
    [SerializeField] GameObject partyBoxOverlay;
    [SerializeField] GameObject buttons;
    
    [SerializeField] BoxHeaderUI boxHeaderUI;

    [SerializeField] List<BoxPokemon> boxSlots;
    [SerializeField] List<BoxPokemon> partySlots;


    public int BoxSelection { get; private set; }
    public int SelectionIndex { get; private set; }
    public Pokemon HeldPokemon { get; set; }
    public bool HoldingPokemon { get => HeldPokemon != null; }



    public event Action OnPokemonSelected;
    public event Action OnBack;

    PokemonParty party;
    PokemonStorage pokemonBoxes;

    private void Start()
    {
        party = PokemonParty.GetPlayerParty();
        pokemonBoxes = PokemonStorage.GetPlayerStorageBoxes();

        BoxReset();

        List<Pokemon> box = pokemonBoxes.GetBoxByIndex(BoxSelection);
        boxHeaderUI.SetBoxNumber(BoxSelection);
        for (int i = 0; i < boxSlots.Count; i++)
        {
            boxSlots[i].SetData(box[i]);
        }

        SetPartyData();

        UpdateHoverSelection(BoxSelection, SelectionIndex);
    }

    public void HandleUpdate()
    {
        if (Input.GetButtonDown("Selection"))
        {
            if (SelectionIndex == PARTY_BUTTON)
            {
                // Open party, move cursor, place shadow and cursor on party member 0
                
                partyBoxOverlay.SetActive(true);
                UpdateHoverSelection(SelectionIndex, FIRST_PARTY_MEMBER);
                SelectionIndex = FIRST_PARTY_MEMBER;
            }
            else if (SelectionIndex == EXIT_BUTTON)
            {
                OnBack?.Invoke();
            }
            else if (SelectionIndex > BANNER_SELECTION && SelectionIndex <= 43)
            {
                if (HoverSpotFull() || HoldingPokemon)
                    OnPokemonSelected?.Invoke();
            }
            else if (SelectionIndex == 44)
            {
                // party close button
                SelectionIndex = 0;
                partyBoxOverlay.SetActive(false);
                UpdateHoverSelection(44, SelectionIndex);
            }
 
        }
        else if (Input.GetButtonDown("GoBack"))
        {
            if (SelectionIndex > EXIT_BUTTON)
            {
                // if in party, close party
                var prev = SelectionIndex;
                SelectionIndex = 0;
                partyBoxOverlay.SetActive(false);
                UpdateHoverSelection(prev, SelectionIndex);
            }
            else if (!HoldingPokemon)
            {
                OnBack?.Invoke();
            }
        }
        else
        {
            int prev = SelectionIndex;
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (30 <= SelectionIndex && SelectionIndex < 34)
                    SelectionIndex = PARTY_BUTTON;
                else if (SelectionIndex == 34 || SelectionIndex == 35)
                    SelectionIndex = EXIT_BUTTON;
                else if (prev < BOX_LIMIT && prev > -1)
                    SelectionIndex += ROW_LIMIT;
                else if (prev > EXIT_BUTTON)
                    SelectionIndex += 2;
                else if (prev == EXIT_BUTTON || prev == PARTY_BUTTON)
                    SelectionIndex = -1;
                else if (prev == BANNER_SELECTION)
                    SelectionIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (prev == EXIT_BUTTON || prev == PARTY_BUTTON)
                    SelectionIndex = BOX_LIMIT - 6;
                else if (prev == BANNER_SELECTION)
                    SelectionIndex = PARTY_BUTTON;
                else if (prev < BOX_LIMIT)
                    SelectionIndex -= ROW_LIMIT;
                else if (prev > EXIT_BUTTON)
                {
                    SelectionIndex -= 2;
                    if (SelectionIndex < FIRST_PARTY_MEMBER)
                        SelectionIndex = prev;
                }
                    
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (prev == -1)
                {
                    // reload box with new pokemon
                    BoxSelection--;
                    if (BoxSelection == -1)
                        BoxSelection = N_BOXES - 1;
                    SwitchBox();
                }
                else if (prev == PARTY_BUTTON)
                    SelectionIndex = EXIT_BUTTON;
                else if (prev == EXIT_BUTTON)
                    SelectionIndex = PARTY_BUTTON;
                else if (prev < BOX_LIMIT || (prev > EXIT_BUTTON && (prev % 2 == 1)))
                    SelectionIndex--;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (prev == -1)
                {
                    // reload box with new pokemon
                    BoxSelection++;
                    if (BoxSelection >= N_BOXES)
                        BoxSelection = 0;
                    SwitchBox();
                    
                }
                else if (prev == PARTY_BUTTON)
                    SelectionIndex = EXIT_BUTTON;
                else if (prev == EXIT_BUTTON)
                    SelectionIndex = PARTY_BUTTON;
                else if (prev > EXIT_BUTTON && (prev % 2 == 1))
                {
                    SelectionIndex = 0;
                    partyBoxOverlay.SetActive(false);
                    UpdateHoverSelection(prev, SelectionIndex);
                    // remove overlay, go back to current box with selection = 0
                }
                else
                    SelectionIndex++;
            }

            SelectionIndex = Mathf.Clamp(SelectionIndex, -1, 44);
            if (SelectionIndex != prev)
            {
                UpdateHoverSelection(prev, SelectionIndex);
            }

        }
    }

    
    public void SetPartyData()
    {
        for (int i = 0; i < party.PokemonList.Count; i++)
            partySlots[i].SetData(party.PokemonList[i]);
        for (int i = party.PokemonList.Count; i < partySlots.Count; i++)
            partySlots[i].SetData(null);

        UpdateHoverSelection(SelectionIndex, SelectionIndex);
    }

    public void BoxReset()
    {
        BoxSelection = 0;
        SelectionIndex = 0;
        HeldPokemon = null;
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
                if (!HoldingPokemon)
                    boxInfoOverlay.ShowDetails(pokemonBoxes.GetPokemon(BoxSelection, currentSelection));
            }
            else
            {
                var current = partySlots[currentSelection - FIRST_PARTY_MEMBER];
                current.HoverSelect();
                if (!HoldingPokemon)
                    boxInfoOverlay.ShowDetails(current.GetPokemon());
            }
            
        }
            
    }
    void SwitchBox()
    {
        List<Pokemon> box = pokemonBoxes.GetBoxByIndex(BoxSelection);
        boxHeaderUI.SetBoxNumber(BoxSelection);
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

    public bool HoverSpotFull()
    {
        if (buttonsList.Contains(SelectionIndex))
        {
            Debug.LogWarning("Checking if button spot is occupied by pokemon");
            return false;
        }

        if (SelectionIndex > EXIT_BUTTON)
        {
            Debug.Log($"{party.PokemonList.Count} pokemon in party, asking for index {SelectionIndex - EXIT_BUTTON}");
            if (SelectionIndex - EXIT_BUTTON > party.PokemonList.Count)
                return false;
            else
                return true;
        }

        return pokemonBoxes.GetPokemon(BoxSelection, SelectionIndex) != null;
    }

    public bool InBox()
    {
        if (!buttonsList.Contains(SelectionIndex))
        {
            if (SelectionIndex > BOX_LIMIT)
                return false;
            else
                return true;
        }

        return false;
    }

    public Pokemon GetHoverPokemon()
    {
        Pokemon poke = null;
        if (InBox())
        {
            poke = pokemonBoxes.GetPokemon(BoxSelection, SelectionIndex);
        }
        else
        {
            
            if (SelectionIndex - FIRST_PARTY_MEMBER >= party.PokemonList.Count)
            {
                return null;
            }
            poke = party.PokemonList[SelectionIndex - FIRST_PARTY_MEMBER];
        }

        if (poke == null)
        {
            Debug.LogWarning("Trying to get null hover pokemon");
        }
        return poke;
    }

    public void ShuffleParty()
    {
        if (SelectionIndex < FIRST_PARTY_MEMBER)
        {
            Debug.LogError("Trying to shuffle party when not in party");
        }

        SetPartyData();


    }

    public void EmptyCurrentSpot()
    {
        if (SelectionIndex > BOX_LIMIT)
        {
            Debug.LogError("Trying to empty when spot is not in box");
        }

        boxSlots[SelectionIndex].Clear();
    }

    public void FillCurrentSpot() 
    {
        boxSlots[SelectionIndex].SetData(HeldPokemon);
        UpdateHoverSelection(SelectionIndex, SelectionIndex);
    }



}
