using UnityEngine;
using System.Collections;

public class InvertGate : Invokable
{
    public Invokable[] ConnectedObjects;

    public void Start()
    {
        transform.localPosition = (transform.localPosition + transform.localScale / 2);
        transform.localScale = new Vector3();
        foreach (Invokable ConnectedObject in ConnectedObjects)
        {
            ConnectedObject.SetActive(!isActive);
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
