using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public string levelName = "LevelName";
    public AudioClip music;
    public AudioClip levelStartClip;
    public AudioClip levelEndClip;
    public GameObject LevelExit;
    public GuiStylePreset levelScreenUI;
    public GuiStylePreset levelExitUI;
    public float musicVolume = 0.2f;
    public GuiStylePreset clockStyle;

    private int portalEnterFrame = 0;
    private int framesPassed = 0;
    private float opacity = 1;
    private AudioSource sound;
    private bool isPlaying = false;
    private Rigidbody2D mover;
    private Camera playerCamera;
    private bool isInPortal = false;
    private AudioSource exitPortal;
    private SpriteRenderer sprite;
    private Player player;
    private float time = 0;

    private bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
    }

    public void Start()
    {
        if (!isEditorMode())
        {
            player = GameObject.FindObjectOfType<Player>();
            playerCamera = player.myCamera;
            sound = playerCamera.gameObject.AddComponent<AudioSource>();
            sound.clip = music;
            sound.loop = true;
            sound.volume = musicVolume;
            mover = player.rigidbody2D;
            mover.Sleep();
            sprite = player.GetComponent<SpriteRenderer>();
            AudioSource.PlayClipAtPoint(levelStartClip, player.InitialSpawn.transform.localPosition, 1);
        }
    }
    
    public string formatTime()
    {
        float seconds = Mathf.Floor(time % 60);
        float minutes = Mathf.Floor(time / 60);
        string actualSeconds = seconds.ToString();
        if (actualSeconds.Length == 1)
        {
            actualSeconds = "0" + actualSeconds;
        }
        return minutes + ":" + actualSeconds;
    }

    public void OnGUI()
    {
        if (!isEditorMode())
        {
            framesPassed++;
            if (framesPassed > 200)
            {
                if (!mover.IsAwake())
                {
                    mover.WakeUp();
                }
                if (!isInPortal)
                {
                    opacity = Mathf.Max(0, opacity - 0.01f);
                }
            }
            else
            {
                mover.Sleep();
            }
            GUI.color = new Color(1, 1, 1, opacity);
            if (framesPassed < 360 || isInPortal)
            {
                Rect size = new Rect(0, 0, Screen.width, Screen.height);
                if (isInPortal)
                {
                    GUI.Box(size, levelName, levelExitUI.Style);
                }
                else
                {
                    GUI.Box(size, levelName, levelScreenUI.Style);
                }
            }
            else
            {
                if (!isPlaying)
                {
                    isPlaying = true;
                    sound.Play();
                }
            }
            if (!isInPortal && isPlaying && !player.Dead)
            {
                time = time + (Time.deltaTime/2);
                GUI.color = new Color(0, 0, 1, 1);
                GUI.Label(new Rect(3, 3, Screen.width, Screen.height), formatTime(),clockStyle.Style);
            }
        }
        else
        {
            LevelEditor edit = GameObject.FindObjectOfType<LevelEditor>();
            if (edit.levelOpen)
            {
                GameObject[] exits = GameObject.FindGameObjectsWithTag("Portal");
                if (exits.Length == 0)
                {
                    GUI.color = Color.red;
                    GUI.Label(new Rect(5, 500, 1000, 100), "WARNING: Level must have an Exit Portal, or problems will arise");
                }
                else
                {
                    this.LevelExit = exits[exits.Length - 1];
                    if (exits.Length > 1)
                    {
                        for (int i = 0; i < exits.Length - 1; i++)
                        {
                            Object.DestroyImmediate(exits[i]);
                        }
                    }
                }
            }
        }
    }

    public void Update()
    {
        if (!isEditorMode())
        {
            LevelExit.transform.eulerAngles = new Vector3(0, 0, framesPassed / 3);
            if ((player.transform.localPosition-LevelExit.transform.localPosition).magnitude < 2 && !isInPortal)
            {
                isInPortal = true;
                player.InPortal = true;
                portalEnterFrame = framesPassed;
                levelName = "";
                sound.volume = 0;
                sound.Stop();
                AudioSource.PlayClipAtPoint(levelEndClip,LevelExit.transform.localPosition,1);
            }
            if (isPlaying && !isInPortal)
            {
                if (player.Dead)
                {
                    sound.pitch = Mathf.Max(0.1f, sound.pitch - 0.005f);
                    sound.volume = Mathf.Max(0, sound.volume - (0.005f * musicVolume));
                }
                else
                {
                    sound.pitch = Mathf.Min(1, sound.pitch + 0.01f);
                    sound.volume = Mathf.Min(musicVolume, sound.volume + 0.01f);
                }
            }
            else if (isInPortal)
            {
                Vector3 offset = LevelExit.transform.localPosition-player.transform.localPosition;
                int update = (framesPassed - portalEnterFrame);
                if (update > 320)
                {
                    opacity = opacity + 0.01f;
                }
                sprite.color = new Color(1f, 1f, 1f, 1f - (update / 600f));
                if (offset.magnitude > 0)
                {
                    Vector3 move = offset.normalized / 100;
                    player.OnGround = true;
                    player.Jumping = false;
                    player.transform.localPosition = player.transform.localPosition + move;
                }
                mover.Sleep();
            }
        }
        else
        {
            transform.localPosition = new Vector3(99999, 99999, 99999);
        }
    }
}
