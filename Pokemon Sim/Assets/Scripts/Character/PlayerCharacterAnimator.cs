using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterAnimator : CharacterAnimator
{
    //public bool IsRunning { get; set; }
    [SerializeField] List<Sprite> runDownSprites;
    [SerializeField] List<Sprite> runUpSprites;
    [SerializeField] List<Sprite> runLeftSprites;
    [SerializeField] List<Sprite> runRightSprites;
    SpriteAnimator runDownAnim;
    SpriteAnimator runUpAnim;
    SpriteAnimator runRightAnim;
    SpriteAnimator runLeftAnim;
    bool wasPrevRunning;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Debug.Log("starting assignment of running sprites");
        runDownAnim = new SpriteAnimator(runDownSprites, spriteRenderer);
        runUpAnim = new SpriteAnimator(runUpSprites, spriteRenderer);
        runLeftAnim = new SpriteAnimator(runLeftSprites, spriteRenderer);
        runRightAnim = new SpriteAnimator(runRightSprites, spriteRenderer);

    }

    new void Update()
    {
        var prevAnim = currAnim;

        if (IsSurfing)
        {
            if (MoveX == 1)
            {
                currAnim = surfRightAnim;
            }
            else if (MoveX == -1)
            {
                currAnim = surfLeftAnim;
            }
            else if (MoveY == 1)
            {
                currAnim = surfUpAnim;
            }
            else if (MoveY == -1)
            {
                currAnim = surfDownAnim;
            }
        }
        else if (IsRunning)
        {
            if (MoveX == 1)
            {
                currAnim = runRightAnim;
            }
            else if (MoveX == -1)
            {
                currAnim = runLeftAnim;
            }
            else if (MoveY == 1)
            {
                currAnim = runUpAnim;
            }
            else if (MoveY == -1)
            {
                currAnim = runDownAnim;
            }

        } 
        else
        {
            if (MoveX == 1)
            {
                currAnim = walkRightAnim;
            }
            else if (MoveX == -1)
            {
                currAnim = walkLeftAnim;
            }
            else if (MoveY == 1)
            {
                currAnim = walkUpAnim;
            }
            else if (MoveY == -1)
            {
                currAnim = walkDownAnim;
            }
        }

        if (currAnim != prevAnim || IsMoving != wasPrevMoving)
        {
            currAnim.Start();
        }
        if (IsJumping)
            spriteRenderer.sprite = currAnim.Frames[3];
        else if (IsMoving)
            currAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currAnim.Frames[0];

        wasPrevMoving = IsMoving;
        wasPrevRunning = IsRunning;
    }

    
}
