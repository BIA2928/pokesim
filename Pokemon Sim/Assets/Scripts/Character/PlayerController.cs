using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Sprite battleSprite;
    [SerializeField] string name;

    const float offsetY = 0.3f;

    private Vector2 input;

    private Character character;

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
        var colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var trigger = collider.GetComponent<IPlayerTriggerable>();
            if (trigger != null)
            {
                trigger.OnPlayerTrigger(this);
                break;
            }
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
            Debug.Log($"Checkpoint 2");
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

    public Character Character
    {
        get => character;
    }

}
