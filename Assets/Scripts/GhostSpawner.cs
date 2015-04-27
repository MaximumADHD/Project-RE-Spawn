using UnityEngine;
using System.Collections;

public class GhostSpawner : MonoBehaviour 
{
    public Monster Ghost;
    private bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
    }

    public void Start()
    {
        if (!isEditorMode())
        {
            Object.Instantiate(Ghost.gameObject, transform.localPosition, new Quaternion());
            transform.localScale = new Vector3();
        }
    }

}
