using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InventoryUIState { ItemSelection, PartySelection, Busy, MoveToForget}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] ItemDescriptionBarUI itemDescriptionBar;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] PocketSelectorUI pocketUI;
    [SerializeField] MoveForgetScreen moveForgetScreen;

    Action<ItemBase> OnItemUsed;
    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;

    MoveBase currMoveToLearn;

    int selectedItem = 0;
    int selectedPocket = 0;
    
    InventoryUIState state;

    const int itemsInViewPort = 7;
    
    const int nPockets = 8;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        UpdateItemSelection();
        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        // Clear exisiting items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetItemsByCategory(selectedPocket))
        {
            var newSlotUI = Instantiate(itemSlotUI, itemList.transform);
            newSlotUI.SetData(itemSlot);
            slotUIList.Add(newSlotUI);
        }
        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        OnItemUsed = onItemUsed;
        Debug.Log("Handling update in inventoryUI");
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevPocket = selectedPocket;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedPocket;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedPocket;

            if (selectedPocket >= nPockets)
                selectedPocket = 0;
            else if (selectedPocket < 0)
                selectedPocket = nPockets - 1;
            selectedItem = MyClamp(selectedItem, 0, inventory.GetItemsByCategory(selectedPocket).Count - 1);

            if (selectedItem != prevSelection)
            {
                UpdateItemSelection();
            }
            if (selectedPocket != prevPocket)
            {
                ResetOnPocketChange();
                UpdateItemList();
                pocketUI.UpdatePocket(selectedPocket);
                UpdateItemSelection();
            }
                

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Debug.Log("partyScreen handles updates");
            Action OnBack = () =>
            {
                ClosePartyScreen();
            };

            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
                
            };
            partyScreen.HandleUpdate(onSelected, OnBack);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Debug.Log("moveScreen handles updates");
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveForgotten(moveIndex));
            };
            Action OnBack = () =>
            {
                Debug.Log("Going back");
                moveForgetScreen.gameObject.SetActive(false);
                partyScreen.gameObject.SetActive(false);
                state = InventoryUIState.ItemSelection;           
            };
            moveForgetScreen.HandleUpdate(onMoveSelected, OnBack);
        }
        
    }

    void UpdateItemSelection()
    {
        var items = inventory.GetItemsByCategory(selectedPocket);
        selectedItem = MyClamp(selectedItem, 0, items.Count - 1);
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].SelectItem();
            }
            else
                slotUIList[i].DeselectItem();
        }

        
        if (items.Count > 0)
        {
            itemDescriptionBar.SetData(items[selectedItem].ItemBase);
            HandleScrolling();
        }

    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPort) return;
        float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    void ResetOnPocketChange()
    {
        itemDescriptionBar.ClearFields();
        selectedItem = 0;
    }

    void OpenPartyScreen()
    {

        state = InventoryUIState.PartySelection;

        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {

        state = InventoryUIState.ItemSelection;
        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator UseItem() 
    {
        state = InventoryUIState.Busy;

        yield return HandleTM();

        // Use item on selected poke
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedPocket);
        if (usedItem == null)
        {
            // Potentially add more info here, use Prof Rowan's voice echoes in your head: theres a time and place for everything
            if (selectedPocket == (int)ItemType.MedicineItem)
                yield return DialogueManager.Instance.ShowDialogue($"It won't have any effect.");
        }
        else
        {
            if (usedItem is MedicineItem)
                yield return DialogueManager.Instance.ShowDialogue($"One {usedItem.Name} was used.");
            OnItemUsed?.Invoke(usedItem);
        }

        ClosePartyScreen();
    }

    IEnumerator HandleTM()
    {
        var item = inventory.GetItemBase(selectedItem, selectedPocket) as TmItem;

        if (item == null)
            yield break;

        Pokemon pokemon = partyScreen.SelectedMember;
        if (pokemon.HasMove(item.Move))
        {
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} already knows {item.Move.Name}.");
            yield break;
        }

        if (!pokemon.Base.CanLearnByTm(item.Move))
        {
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} cannot learn {item.Move.Name}.");
            yield break;
        }

        if (pokemon.Moves.Count == PokemonBase.MaxNMoves)
        {
            
            yield return AskToForget(pokemon, item.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }
        else
        {
            pokemon.LearnMove(item.Move);
            yield return DialogueManager.Instance.ShowDialogue($"{pokemon.Base.Name} learned {item.Move.Name}!");
        }

    }

    IEnumerator AskToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        currMoveToLearn = newMove;
        Dialogue dialogue = new Dialogue();
        dialogue.Lines.Add($"{pokemon.Base.Name} wants to learn {newMove.Name}.");
        dialogue.Lines.Add($"But {pokemon.Base.Name} already knows four moves.");
        dialogue.Lines.Add($"Select a move for {pokemon.Base.Name} to forget.");
        yield return DialogueManager.Instance.ShowDialogueContinuous(dialogue);
        yield return new WaitUntil(() => DialogueManager.Instance.IsShowing == false);
        moveForgetScreen.gameObject.SetActive(true);
        moveForgetScreen.Init();
        moveForgetScreen.SetMoveData(pokemon.Moves, newMove);
        state = InventoryUIState.MoveToForget;
    }

    IEnumerator OnMoveForgotten(int moveIndex)
    {
        moveForgetScreen.gameObject.SetActive(false);
        Debug.Log($"Learning {currMoveToLearn.Name}");
        var poke = partyScreen.SelectedMember;
        poke.ReplaceMove(poke.Moves[moveIndex], new Move(currMoveToLearn));
        yield return DialogueManager.Instance.ShowDialogue($"{poke.Base.Name} forgot a move and learned {currMoveToLearn.Name}!");
        currMoveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }


    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItemBase(selectedItem, selectedPocket);
        if (GameController.i.GameState == GameState.InBattle)
        {
            if (!item.CanUseInBattle)
            {
                // Can't use item
                Dialogue dialogue = new Dialogue();
                dialogue.Lines.Add($"Professor Rowan's words echo in your ears...");
                dialogue.Lines.Add($"There's a time and place for everything!\nBut not now.");
                yield return DialogueManager.Instance.ShowDialogue(dialogue);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            if (!item.CanUseOutsideBattle)
            {
                // Can't use item
                Dialogue dialogue = new Dialogue();
                dialogue.Lines.Add($"Professor Rowan's words echo in your ears...");
                dialogue.Lines.Add($"There's a time and place for everything!\nBut not now.");
                yield return DialogueManager.Instance.ShowDialogue(dialogue);
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        if (selectedPocket == (int)ItemType.Pokeball)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();
            if (item is TmItem)
            {
                // Show tm useable or not
                partyScreen.ShowIfTMUsable(item as TmItem);
            }
        }
    }

    public static int MyClamp(int n, int min, int max)
    {
        if (max < min || n < min)
            return min;
        if (n > max)
            return max;

        return n;
    }
}


