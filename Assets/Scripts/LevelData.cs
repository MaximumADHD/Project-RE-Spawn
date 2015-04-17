using UnityEngine;
using System.Collections;

public class LevelData : MonoBehaviour 
{
    public string levelName = "LevelName";
    public AudioClip music;
    public GUIStyle levelScreenModifier;
    private int framesPassed = 0;
    private float opacity = 1;

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
    }
}
