using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public string levelName = "LevelName";
    public AudioClip music;
    public GUIStyle levelScreenModifier;
    public float musicVolume = 0.2f;

    private int framesPassed = 0;
    private float opacity = 1;
    private AudioSource sound;
    private bool isPlaying = false;
    private Player player;
    private Camera playerCamera;

    public void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        playerCamera = player.myCamera;
        sound = playerCamera.gameObject.AddComponent<AudioSource>();
    }

    public void OnGUI()
    {
        framesPassed++;
        if (framesPassed > 200)
        {
            opacity = opacity - 0.01f;
        }
        GUI.color = new Color(1, 1, 1, opacity);
        if (framesPassed < 360)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), levelName, levelScreenModifier);
        }
        else
        {
            if (!isPlaying)
            {
                isPlaying = true;
                sound.clip = music;
                sound.loop = true;
                sound.volume = musicVolume;
                sound.Play();
            }
        }
    }

    public void Update()
    {
        if (isPlaying)
        {
            if (player.Dead)
            {
                sound.pitch = Mathf.Max(0.1f, sound.pitch - 0.005f);
            }
            else
            {
                sound.pitch = Mathf.Min(1, sound.pitch + 0.01f);
            }
        }
    }
}
