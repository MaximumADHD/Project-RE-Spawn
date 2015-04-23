// Max G 2015 <3
// SpawnLocation.cs

using UnityEngine;
using System.Collections;

public class SpawnLocation : Invokable
{
    public bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
    }

    public void Start()
    {
        if (!isEditorMode())
        {
            transform.localPosition = (transform.localPosition + transform.localScale / 2);
            transform.localScale = new Vector3();
        }
    }

    public override void OnActiveChanged(bool newState)
    {
        // This has to be here, but we don't do anything with it :P
    }
}
