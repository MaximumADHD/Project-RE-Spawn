// Max G 2015 <3
// BaseInteractive.cs

using UnityEngine;
using System;
using System.Collections;

public abstract class Interactive : MonoBehaviour 
{
    public float range = 4;
    public float coolDown = 1;
    public string toolTip = "Type a message which will be shown when the player is near the object.";
    public GuiStylePreset InteractUI;
    private float updated = 0;

    abstract public void OnInteract();

    public void OnGUI()
    {
        if (enabled)
        {
            Player Player = GameObject.FindObjectOfType<Player>();
            if (Player && !Player.Dead)
            {
                float dist = Vector3.Distance(Player.transform.localPosition,transform.localPosition);
                if ((dist*3) <= range)
                {
                    float interact = Input.GetAxis("Interact");
                    if (updated >= coolDown)
                    {
                        if (interact == 1)
                        {
                            updated = 0;
                            OnInteract();
                        }
                        else
                        {
                            GUI.Label(new Rect(0, 0, Screen.width,Screen.height), toolTip,InteractUI.Style);
                        }
                    }
                    else
                    {
                        updated = updated + .015f;
                    }
                }
            }
        }

    }
}
