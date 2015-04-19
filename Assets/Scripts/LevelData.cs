using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public string levelName = "LevelName";
    public AudioClip music;
    public GUIStyle levelScreenModifier;
    public GameObject LevelExit;
    public Player player;
    public float musicVolume = 0.2f;

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

    public void Start()
    {
        playerCamera = player.myCamera;
        sound = playerCamera.gameObject.AddComponent<AudioSource>();
        sound.clip = music;
        sound.loop = true;
        sound.volume = musicVolume;
        exitPortal = LevelExit.GetComponent<AudioSource>();
        mover = player.rigidbody2D;
        mover.Sleep();
        sprite = player.GetComponent<SpriteRenderer>();
    }

    public void OnGUI()
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
                opacity = opacity - 0.01f;
            }
        }
        else
        {
            mover.Sleep();
        }
        GUI.color = new Color(1, 1, 1, opacity);
        if (framesPassed < 360 || isInPortal)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), levelName, levelScreenModifier);
        }
        else
        {
            if (!isPlaying)
            {
                isPlaying = true;
                sound.Play();
            }
        }
    }

    public void Update()
    {
        LevelExit.transform.eulerAngles = new Vector3(0, 0, framesPassed / 3);
        if ((player.transform.localPosition-LevelExit.transform.localPosition).magnitude < 2 && !isInPortal)
        {
            isInPortal = true;
            portalEnterFrame = framesPassed;
            levelName = "";
            sound.volume = 0;
            sound.Stop();
            exitPortal.Play();
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
            if (update > 300)
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
}
