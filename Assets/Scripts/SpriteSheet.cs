using UnityEngine;
using System.Collections;

public class SpriteSheet : MonoBehaviour 
{
    public Sprite[] frames = { };
    private int currentFrame = 0;
    private int update = 0;
    
    public Sprite NextFrame()
    {
        update++;
        if (update >= 6 * (8/frames.Length))
        {
            bool canUpdate = true; // Gets set to false if we're paused.
            LevelData data = GameObject.FindObjectOfType<LevelData>();
            if (data != null)
            {
                if (data.isPaused)
                {
                    canUpdate = false;
                }
            }
            if (canUpdate)
            {
                update = 0;
                currentFrame = (currentFrame + 1) % frames.Length;
            }
        }
        return frames[currentFrame];
    }

    public void Reset()
    {
        currentFrame = 0;
    }
}
