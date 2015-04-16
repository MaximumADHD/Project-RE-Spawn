// Max G 2015 <3
// LeverScript.cs

using UnityEngine;
using System.Collections;

public class LeverScript : Interactive
{
    public Invokable[] ConnectedObjects;
    public bool Activated = false;
    public Sprite OnSprite;
    public Sprite OffSprite;
    public AudioClip leverClick;

    private SpriteRenderer sprite;

    private void UpdateSprite()
    {
        if (Activated)
        {
            sprite.sprite = OnSprite;
        }
        else
        {
            sprite.sprite = OffSprite;
        }
    }
    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public override void OnInteract()
    {
        foreach (Invokable ConnectedObject in ConnectedObjects)
        {
            ConnectedObject.ToggleActive();
        }
        Activated = !Activated;
        UpdateSprite();
    }
}
