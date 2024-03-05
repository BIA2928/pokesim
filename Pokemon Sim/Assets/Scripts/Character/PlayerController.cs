using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Sprite battleSprite;
    [SerializeField] string name;

    private Vector2 input;

    private Character character;

    public event Action OnWildEncounter;
    public event Action<Collider2D> OnTrainerEncounter;

    void Awake()
    {
        character = this.GetComponent<Character>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {

        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
                
            }
        }

        character.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void OnMoveOver()
    {
        CheckForEncounters();
        CheckSeenByTrainer();
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.1f, GameLayers.i.GrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1,10) == 9)
            {
                character.Animator.IsMoving = false;
                //character.HandleUpdate();
                OnWildEncounter();
            }
        }
    }

    private void CheckSeenByTrainer()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.15f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnTrainerEncounter?.Invoke(collider);
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactingPos = transform.position + facingDir;


        var collider = Physics2D.OverlapCircle(interactingPos, 0.5f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactive>()?.Interact(transform);
            character.Stop();
        }
    }
    public string Name
    {
        get { return name; }
    }

    public Sprite BattleSprite
    {
        get { return battleSprite; }
    }

}
