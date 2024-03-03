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

    public float MoveX { get; set; }
    public float MoveY { get; set; }

    public bool IsMoving { get; set; }

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

        currAnim = walkDownAnim;
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

        if (IsMoving)
            currAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currAnim.Frames[0];

        wasPrevMoving = IsMoving;
    }
}
