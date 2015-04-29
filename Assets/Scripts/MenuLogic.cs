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
        MenuLogic logic = FindObjectOfType<MenuLogic>();
        if (logic.playIntro)
        {
            AudioSource.PlayClipAtPoint(music, myCamera.transform.localPosition, 1);
            Animation hatAnim = casualHatGames.gameObject.GetComponent<Animation>();
            hatAnim.Play();
        }
        else
        {
            showingLogo = true;
        }
    }


    public void Update()
    {
        // Update Time Elapse
        actualTimeElapsed = actualTimeElapsed + Time.deltaTime;
        timeElapsed = actualTimeElapsed;
        // Do things
        if (timeElapsed > 5 && !showingLogo && !inLevelMenu)
        {
            showingLogo = true;
        }
        float goal = 0;
        if (showingLogo)
        {
            goal = 0.5f;
        }
        logoTransparency = glideTorwards(logoTransparency,goal,0.01f);
        logo.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
        guy.color = new Color(0.5f, 0.5f, 0.5f, logoTransparency);
        currentFrame = (currentFrame+1)%5;
        guy.texture = guyAnim[currentFrame];
    }

}
