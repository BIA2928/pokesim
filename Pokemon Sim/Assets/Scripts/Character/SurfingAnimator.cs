using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfingAnimator : MonoBehaviour
{
    RectTransform rectTransform;
    SpriteRenderer spriteRenderer;
    [SerializeField] List<Sprite> surfDownSprites;
    SpriteAnimator surfDownAnim;
    [SerializeField] List<Sprite> surfUpSprites;
    SpriteAnimator surfUpAnim;
    [SerializeField] List<Sprite> surfLeftSprites;
    SpriteAnimator surfLeftAnim;
    [SerializeField] List<Sprite> surfRightSprites;
    SpriteAnimator surfRightAnim;
    [SerializeField] List<Vector2> offsets;

    SpriteAnimator currAnim;

    public bool IsShowing { get; set; }
    public bool IsMoving { get; set; }
    bool wasPrevMoving;
    public float MoveX { get; set; }
    public float MoveY { get; set; }


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<RectTransform>();
        surfDownAnim = new SpriteAnimator(surfDownSprites, spriteRenderer);
        surfRightAnim = new SpriteAnimator(surfRightSprites, spriteRenderer);
        surfUpAnim = new SpriteAnimator(surfUpSprites, spriteRenderer);
        surfLeftAnim = new SpriteAnimator(surfLeftSprites, spriteRenderer);
    }

    public void Show()
    {
        spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
    // Update is called once per frame
    public void HandleUpdate()
    {
        var prevAnim = currAnim;
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

        if ((currAnim != prevAnim || IsMoving != wasPrevMoving) && IsShowing)
        {
            SetOffset();
            currAnim.Start();
        }

        if (IsMoving)
            currAnim.HandleUpdate();
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

    public void SetOffset()
    {
        if (MoveY == -1)
            rectTransform.localPosition = offsets[0];
        else if (MoveY == 1)
            rectTransform.localPosition = offsets[1];
        else if (MoveX == 1)
            rectTransform.localPosition = offsets[3];
        else if (MoveX == -1)
            rectTransform.localPosition = offsets[2];
    }

    public void SetShowing(bool showing)
    {
        IsShowing = showing;
        spriteRenderer.enabled = showing;
    }

    
}
