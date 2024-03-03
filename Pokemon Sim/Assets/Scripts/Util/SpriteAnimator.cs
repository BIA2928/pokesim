using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currFrame;
    float timer;

    public List<Sprite> Frames
    {
        get { return frames; }
    }

    public SpriteAnimator (List<Sprite> frames, SpriteRenderer sR, float frameRate=0.16f)
    {
        this.frames = frames;
        this.frameRate = frameRate;
        spriteRenderer = sR;
    }

    public void Start()
    {
        currFrame = 0;
        timer = 0f;
        spriteRenderer.sprite = frames[currFrame];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currFrame = (currFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currFrame];
            timer -= frameRate;
        }
    }

}
