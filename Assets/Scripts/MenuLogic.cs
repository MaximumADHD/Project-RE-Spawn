using UnityEngine;
using System.Collections;

public class MenuLogic : MonoBehaviour 
{
    public AudioClip music;
    public Camera myCamera;
    public GUITexture logo;
    public GUITexture guy;
    public Texture[] guyAnim = new Texture[5];
    public GUITexture casualHatGames;
    
    public bool playIntro = true;
    public float timeElapsed = 0;
    private float actualTimeElapsed = 0;
    private float logoTransparency = 0;
    private bool showingLogo = false;
    private bool inLevelMenu = false;
    private int currentFrame = 0;
    private float lastUpdate = 0;
    private bool inPlayMenu = false;

    private float glideTorwards(float val, float desired, float increment)
    {
        // Takes a number, and tries to move it to a desired value using +/- increment and Mathf.Max/Min
        if (val > desired)
        {
            return Mathf.Max(val - increment, desired);
        }
        else if (val < desired)
        {
            return Mathf.Min(val + increment, desired);
        }
        else
        {
            return desired;
        }
    }

    public void Start()
    {
        AudioSource.PlayClipAtPoint(music, myCamera.transform.localPosition, 1);
        if (playIntro)
        {
            Animation hatAnim = casualHatGames.gameObject.GetComponent<Animation>();
            hatAnim.Play();
        }
        else
        {
            showingLogo = true;
        }
    }

    public void OnGUI()
    {
        if (!inPlayMenu && showingLogo)
        {
            bool pressingPlay = GUI.Button(new Rect(Screen.width * 0.2f, 300, 200, 60), "Start Game");
            if (pressingPlay)
            {
                showingLogo = false;
                inPlayMenu = true;
            }
        }
    }

    public void Update()
    {
        // Update Time Elapse
        actualTimeElapsed = actualTimeElapsed + Time.deltaTime;
        timeElapsed = actualTimeElapsed;
        // Do things
        if (timeElapsed > 5 && !showingLogo && !inPlayMenu)
        {
            showingLogo = true;
        }
        float goal = 0;
        if (showingLogo)
        {
            goal = 0.5f;
        }
        Debug.Log(goal);
        logoTransparency = glideTorwards(logoTransparency, goal, 0.1f);
        logo.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
        guy.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
        if ((timeElapsed - lastUpdate) > 0.07)
        {
            lastUpdate = timeElapsed;
            currentFrame = (currentFrame + 1) % 8;
            guy.texture = guyAnim[currentFrame];
        }  
    }
}
