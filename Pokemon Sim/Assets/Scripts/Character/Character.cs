using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public float ms; //movement speed
    public bool IsMoving { get; private set; }
    private CharacterAnimator animator;

    public float OffsetY { get; private set; } = 0.3f;
    public CharacterAnimator Animator
    {
        get => animator;
    }

    private void Awake()
    {
        animator = this.GetComponent<CharacterAnimator>();
        SetPosAndSnapToTile(transform.position);
    }

    public void SetPosAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;
        transform.position = pos;

    }

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver=null)
    {

        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if (!IsPathClear(targetPos))
        {
            Debug.Log($"position {targetPos} is not walkable!");
            yield break;
        }

            

        IsMoving = true;
        HandleUpdate();

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, ms * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;
        if (OnMoveOver == null)
        {
            HandleUpdate();
        }
        //HandleUpdate();

        OnMoveOver?.Invoke();
    }


    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return !(Physics2D.OverlapCircle(targetPos, 0.1f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null);
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            
            animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);

            
        }
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        var collision = Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1,
            GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer);
        if (collision)
            Debug.Log(collision.collider.name);
        return (!collision);
    }

    public void Stop()
    {
        this.IsMoving = false;
        HandleUpdate();
    }

}
