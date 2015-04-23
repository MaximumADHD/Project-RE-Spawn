// Max G 2015 <3
// InvertGate.cs

using UnityEngine;
using System.Collections;

public class InvertGate : Invokable
{
    public Invokable[] ConnectedObjects;

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
            foreach (Invokable ConnectedObject in ConnectedObjects)
            {
                ConnectedObject.SetActive(!isActive);
            }
        }
    }

    public override void OnActiveChanged(bool newState)
    {
        foreach (Invokable ConnectedObject in ConnectedObjects)
        {
            ConnectedObject.SetActive(!newState);
        }
    }
}
