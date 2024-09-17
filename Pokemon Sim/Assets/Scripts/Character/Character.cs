using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    const float RUNNING_MULTIPLIER = 1.25f;
    public float ms; //movement speed
    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }
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

    public IEnumerator Move(Vector2 moveVector, Action OnMoveOver=null, bool autoMove=false, bool isPlayer=false, bool checkCollisions=true)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVector.y, -1f, 1f);

        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        var ledge = CheckForLedge(targetPos);
        if (ledge != null) 
        {
            Debug.Log("checkpoint 0");
            if (ledge.TryToJump(this, moveVector))
                yield break;
        }
        if (checkCollisions && !IsPathClear(targetPos))
        {
            if (isPlayer)
            {
                AudioManager.i.PlaySFX(AudioID.Bump);
            }
                
            yield break;
        }

        if (animator.IsSurfing && Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.Water) == null)
        {
            // Trying to get out of water
            animator.SetSurfing(false);
            AudioManager.i.PlaySFX(AudioID.Jump);
            yield return transform.DOJump(targetPos, 0.15f, 1, 0.5f).WaitForCompletion();
            AudioManager.i.StopSurfMusic();
            yield break;
        }
        
        IsRunning = false;
        if (isPlayer)
        {
            if (Input.GetKey(KeyCode.X)) 
            {
                IsRunning = true;
            }
        }
        IsMoving = true;
        HandleUpdate();

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            float factor = IsRunning ? ms * RUNNING_MULTIPLIER : ms;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, factor * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;
        IsRunning = false;
        if (OnMoveOver == null || autoMove)
        {
            HandleUpdate();
        }
        OnMoveOver?.Invoke();
    }


    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
        animator.IsRunning = IsRunning;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return !(Physics2D.OverlapCircle(targetPos, 0.1f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null);
    }

    Ledge CheckForLedge(Vector3 targetPos)
    {
        Debug.DrawLine(transform.position, targetPos, Color.green, 2.0f, false);
        var collider = Physics2D.OverlapCircle(targetPos, 0.15f, GameLayers.i.Ledges);
        if (collider == null)
        {
            return null;
        }
        Debug.Log($"collider is null at {targetPos}");
        return collider.GetComponent<Ledge>();
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

    public void Turn(FacingDirection facingDirection)
    {
        animator.SetFacingDirection(facingDirection);
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;
        var collisionLayers = GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer;
        if (!animator.IsSurfing)
            collisionLayers |= GameLayers.i.Water;
        var collision = Physics2D.BoxCast(transform.position + dir, new Vector2(0.15f, 0.15f), 0f, dir, 
            diff.magnitude - 1, collisionLayers);
        return (!collision);
    }

    public void Stop()
    {
        if (animator is PlayerCharacterAnimator animator1)
            animator1.StandStill();
        IsMoving = false;
        IsRunning = false;
        HandleUpdate();
    }

}
