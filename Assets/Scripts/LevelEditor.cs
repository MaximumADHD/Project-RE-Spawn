using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class LevelEditor : MonoBehaviour 
{
    public GameObject[] objects;
    public GameObject highlighter;
    public Camera myCamera;
    private int currentSprite = 0;
    private bool levelOpen = false;
    private bool inputtingNew = false;
    private bool inputtingOpen = false;
    private string levelName = "LEVEL";
    private string currentPath;
    private float zoom = -10;
    private SpriteRenderer sprite;
    private string mode = "None";
    private int flipDebounce = 0;
    private float displayTime = 0;
    private string displayMsg = "";

    public void OnGUI()
    {
        if (displayTime > 0)
        {
            GUI.Label(new Rect(5, Screen.height/4, Screen.width, Screen.height), displayMsg);
            displayTime = displayTime - Time.deltaTime;
        }
        if (levelOpen)
        {
            GUI.Label(new Rect(5, 3, 100, 30), "GameObject:");
            GUI.Label(new Rect(5, 60, 100, 35), "Mode:");
            GUI.Label(new Rect(5, 120, 150, 500), "Editor Controls:\n\n(WASD/Arrow Keys)\nMove Camera\n\n(Left Shift)\nFast Camera\n\n(MouseWheel Up/Down)\nZoom Camera\n\n(Left Mouse)\nPerform an action based on the current mode.\n\n(Q)\nChange Block\n\n(E)\nChange Mode\n\n(R)\nSave World");
            GUI.Button(new Rect(5, 20, 150, 35), objects[currentSprite].name);
            GUI.Button(new Rect(5, 80, 150, 35), mode);
        }
        else
        {
            if (inputtingNew)
            {
                GUI.Label(new Rect(5, 3, 200, 30), "Enter Level Name:");
                levelName = GUI.TextField(new Rect(5, 23, 100, 20), levelName);
                bool isPressing = GUI.Button(new Rect(5, 46, 100, 20), "Create");
                bool isCanceled = GUI.Button(new Rect(5, 69, 100, 20), "Cancel");
                if (isPressing)
                {
                    if (levelName.Length > 0)
                    {
                        string path = Application.dataPath + "/Levels/" + levelName + ".unity";
                        if (!File.Exists(path))
                        {
                            Debug.Log(path);
                            currentPath = path;
                            levelOpen = true;
                        }
                        else
                        {
                            displayMsg = "Error: A level with this name already exists.";
                            displayTime = 2;
                        }
                    }
                    else
                    {
                        displayMsg = "Error: Level name cannot be blank";
                        displayTime = 2;
                    }
                }
                else if (isCanceled)
                {
                    inputtingNew = false;
                }
            }
            else
            {
                bool pressingNew = GUI.Button(new Rect(5, 3, 100, 30), "New Level");
                bool pressingOpen = GUI.Button(new Rect(5, 33, 100, 30), "Open Level");
                if (pressingNew)
                {
                    inputtingNew = true;
                }
                else if (pressingOpen)
                {
                    inputtingOpen = true;
                }
            }

        }
    }

    public void InputUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSprite = (currentSprite + 1) % objects.Length;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (mode.Equals("None"))
            {
                mode = "Create";
                sprite.color = new Color(0, 1, 0, 1);
            }
            else if (mode.Equals("Create"))
            {
                mode = "Destroy";
                sprite.color = new Color(1, 0, 0, 1);
            }
            else if (mode.Equals("Destroy"))
            {
                mode = "Flip";
                flipDebounce = 10;
                sprite.color = new Color(1, 1, 0, 1);
            }
            else if (mode.Equals("Flip"))
            {
                mode = "None";
                sprite.color = new Color(1, 1, 1, 0);
            }
        }
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
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom = Mathf.Min(-3,Mathf.Max(-15,zoom + (scroll*3)));
        Vector3 cur = myCamera.transform.localPosition;
        myCamera.transform.localPosition = new Vector3(cur.x + x, cur.y + y, zoom);
        if (Input.GetKeyDown(KeyCode.R))
        {
            EditorApplication.SaveScene(currentPath);
            displayMsg = "Saved...";
            displayTime = 1;
        }
    }

    public List<GameObject> GetMouseHits()
    {
        List<GameObject> hit = new List<GameObject>();
        Ray mouseOut = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 pos = new Vector3(Mathf.Ceil(mouseOut.origin.x-0.5f), Mathf.Ceil(mouseOut.origin.y-0.5f));
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in objs)
        {
            if (obj.transform.localPosition == pos && !obj.tag.Equals("LevelEdit"))
            {
                hit.Add(obj);
            }
        }
        return hit;
    }

    public void ModeUpdate()
    {
        Ray mouseOut = myCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 pos = new Vector3(Mathf.Ceil(mouseOut.origin.x-0.5f), Mathf.Ceil(mouseOut.origin.y-0.5f));
        highlighter.transform.localPosition = pos;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            List<GameObject> hit = GetMouseHits();
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
            else if (mode.Equals("Flip"))
            {
                flipDebounce++;
                if (flipDebounce >= 10)
                {
                    flipDebounce = 0;
                    foreach (GameObject obj in hit)
                    {
                        Vector3 cur = obj.transform.localScale;
                        obj.transform.localScale = new Vector3(-cur.x, cur.y, cur.z);
                    }
                }

            }
        }
    }

    public void Start()
    {
        sprite = highlighter.GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0.25f);
        myCamera.transform.localPosition = new Vector3(0, 0, -10);
    }

    public void Update()
    {
        if (levelOpen)
        {
            CameraUpdate();
            InputUpdate();
            ModeUpdate();
        }
        else
        {
            sprite.color = new Color(0, 0, 0, 0);
        }
    }

}
