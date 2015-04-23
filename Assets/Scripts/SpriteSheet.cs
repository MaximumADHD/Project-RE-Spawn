using UnityEngine;
using System.Collections;

public class SpriteSheet : MonoBehaviour 
{
    public Sprite[] frames = { };
    private int currentFrame = 0;
    
    public Sprite NextFrame()
    {
        Debug.Log(frames.Length);
        currentFrame = (currentFrame + 1) % frames.Length;
        return frames[currentFrame];
    }

    public void Reset()
    {
        currentFrame = 0;
    }
}
