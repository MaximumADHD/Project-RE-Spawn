using UnityEngine;
using System.Collections;

public class ToggleBlock : Invokable
{
    private SpriteRenderer sprite;

    public override void OnActiveChanged(bool newState)
    {;
        collider2D.enabled = newState;
        if (newState)
        {
            sprite.color = new Color(1, 1, 1, 1);
        }
        else
        {
            sprite.color = new Color(1, 1, 1, 0);
        }
    }

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        OnActiveChanged(isActive);
    }

    public void Update()
    {
        OnActiveChanged(isActive);
    }
}
