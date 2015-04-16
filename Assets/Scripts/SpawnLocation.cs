// Max G 2015 <3
// SpawnLocation.cs

using UnityEngine;
using System.Collections;

public class SpawnLocation : Invokable
{
    public void Start()
    {
        transform.localPosition = (transform.localPosition + transform.localScale / 2);
        transform.localScale = new Vector3();
    }

    public override void OnActiveChanged(bool newState)
    {
        if (newState)
        {
            Debug.Log(name + " is activated");
        }
        else
        {
            Debug.Log(name + " is deactivated");
        }
    }
}
