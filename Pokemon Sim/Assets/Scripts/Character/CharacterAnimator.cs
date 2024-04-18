using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    //Parameters
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    public float MoveX { get; set; }
    public float MoveY { get; set; }

    public bool IsMoving { get; set; }

    public bool IsJumping { get; set; }

    //States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currAnim;
    bool wasPrevMoving;

    //Refernces
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);
    }

    public FacingDirection DefaultDirection
    {
        get { return defaultDirection; }
    }

    private void Update()
    {
        var prevAnim = currAnim;

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
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Left)
        {
            MoveX = -1;
        }
        else if (dir == FacingDirection.Right)
        {
            MoveX = 1;
        }
        else if (dir == FacingDirection.Up)
        {
            MoveY = 1;
        }
        else
        {
            MoveY = -1;
        }
    }
}

public enum FacingDirection
{
    Up, Down, Left, Right
}
