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
    [SerializeField] List<Sprite> surfLeftSprites;
    [SerializeField] List<Sprite> surfRightSprites;
    [SerializeField] List<Sprite> surfUpSprites;
    [SerializeField] List<Sprite> surfDownSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;
    public float MoveX { get; set; }
    public float MoveY { get; set; }

    public bool IsMoving { get; set; }

    public bool IsRunning { get; set; }

    public bool IsTurning { get; set; }

    public bool IsJumping { get; set; }

    public bool IsSurfing { get; set; }

    //States
    protected SpriteAnimator walkDownAnim;
    protected SpriteAnimator walkUpAnim;
    protected SpriteAnimator walkRightAnim;
    protected SpriteAnimator walkLeftAnim;
    protected SpriteAnimator surfDownAnim;
    protected SpriteAnimator surfUpAnim;
    protected SpriteAnimator surfRightAnim;
    protected SpriteAnimator surfLeftAnim;

    protected SpriteAnimator currAnim;
    protected bool wasPrevMoving;

    //Refernces
    protected SpriteRenderer spriteRenderer;

    protected void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);


        surfDownAnim = new SpriteAnimator(surfDownSprites, spriteRenderer);
        surfRightAnim = new SpriteAnimator(surfRightSprites, spriteRenderer);
        surfUpAnim = new SpriteAnimator(surfUpSprites, spriteRenderer);
        surfLeftAnim = new SpriteAnimator(surfLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);
    }

    public FacingDirection DefaultDirection
    {
        get { return defaultDirection; }
    }

    protected void Update()
    {
        var prevAnim = currAnim;
        
        if (IsSurfing)
        {
            if (MoveX == 1)
            {
                currAnim =  surfRightAnim;
            }
            else if (MoveX == -1)
            {
                currAnim =  surfLeftAnim;
            }
            else if (MoveY == 1)
            {
                currAnim = surfUpAnim;
            }
            else if (MoveY == -1)
            {
                currAnim =  surfDownAnim;
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

    public void SetSurfing(bool surfing)
    {
        IsSurfing = surfing;
    }

    public void Turn(FacingDirection dir)
    {
        
        if (dir == FacingDirection.Left)
        {
            Debug.Log("Turning left animator");
            spriteRenderer.sprite = walkLeftSprites[0];        
        }
        else if (dir == FacingDirection.Right)
        {
            Debug.Log("Turning right animator");
            spriteRenderer.sprite = walkRightSprites[0];
        }
        else if (dir == FacingDirection.Up)
        {
            Debug.Log("Turning up animator");
            spriteRenderer.sprite = walkUpSprites[0];
        }
        else
        {
            Debug.Log("Turning down animator");
            spriteRenderer.sprite = walkDownSprites[0];
        }
    }

}

public enum FacingDirection
{
    Up, Down, Left, Right
}
