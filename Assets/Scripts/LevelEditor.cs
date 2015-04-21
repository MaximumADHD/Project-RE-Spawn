using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour 
{
    public GameObject[] objects;
    public Camera myCamera;
    private int currentSprite = 0;
    private bool guiMouseDown = false;
    private bool mouseDown = false;
    private string mode = "Create";

    public void OnGUI()
    {
        GUI.Label(new Rect(5, 3, 100, 30), "GameObject:");
        GUI.Label(new Rect(5, 60, 100, 35), "Mode:");
        bool isPressingItemSelect = GUI.Button(new Rect(5, 20, 100, 35), objects[currentSprite].name);
        bool isPressingMode = GUI.Button(new Rect(5, 80, 100, 35), mode);
        if (!Input.GetMouseButtonDown(0) && guiMouseDown)
        {
            guiMouseDown = false;
        }
        if (isPressingItemSelect && !guiMouseDown)
        {
            guiMouseDown = true;
            currentSprite = (currentSprite + 1) % objects.Length;
        }
        else if (isPressingMode && !guiMouseDown)
        {
            guiMouseDown = true;
            if (mode.Equals("Create"))
            {
                mode = "Destroy";
            }
            else
            {
                mode = "Create";
            }
        }
    }

    public void Start()
    {
        myCamera.transform.localPosition = new Vector3(0, 0, -10);
    }

    public void CameraUpdate()
    {
        float x = 0;
        float y = 0;
        float speed = 0.08f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 0.4f;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            y = speed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            y = -speed;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            x = speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            x = -speed;
        }
        myCamera.transform.Translate(new Vector3(x, y, 0));
    }

    public List<GameObject> getMouseHits()
    {
        List<GameObject> hit = new List<GameObject>();
        Ray mouseOut = myCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mouseOut.origin,mouseOut.direction,Color.red);
        Vector3 pos = new Vector3(Mathf.Ceil(mouseOut.origin.x), Mathf.Ceil(mouseOut.origin.y));
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objs)
        {
            if (obj.transform.localPosition == pos)
            {
                hit.Add(obj);
            }
        }
        return hit;
    }

    public void Update()
    {
        CameraUpdate();
        if (Input.GetKey(KeyCode.Mouse0) && !guiMouseDown)
        {
            Ray mouseOut = myCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 pos = new Vector3(Mathf.Ceil(mouseOut.origin.x), Mathf.Ceil(mouseOut.origin.y));
            List<GameObject> hit = getMouseHits();
            if (mode.Equals("Create"))
            {
                if (hit.Count == 0)
                {
                    GameObject clone = (GameObject)Object.Instantiate(objects[currentSprite], pos, new Quaternion());
                    clone.name = objects[currentSprite].name;
                }
            }
            else if (mode.Equals("Destroy"))
            {
                foreach (GameObject obj in hit)
                {
                    Object.DestroyImmediate(obj);
                }
            }
        }
    }

}
