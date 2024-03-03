using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public LayerMask interactableLayer;
    public float ms; //movement speed
    public bool isMoving;
    private Vector2 input;

    private CharacterAnimator animator;

    public event Action OnWildEncounter;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void HandleUpdate()
    {

        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.MoveX = input.x;
                animator.MoveY = input.y;

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
                
            }
        }

        animator.IsMoving = isMoving;

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    IEnumerator Move(Vector3 targetPos)
    {

        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, ms * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return !(Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) != null);
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.1f, grassLayer) != null)
        {
            if(UnityEngine.Random.Range(1,10) == 9)
            {
                animator.IsMoving = false;
                OnWildEncounter();
            }
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.MoveX, animator.MoveY);
        var interactingPos = transform.position + facingDir;


        var collider = Physics2D.OverlapCircle(interactingPos, 0.5f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactive>()?.Interact();
        }
    }
}
