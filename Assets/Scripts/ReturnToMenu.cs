// Max G <3
// The purpose of this script is to load the current version of the menu level additively to the world
// The MenuLogic checks to see if this "ReturnToMenu" object exists. If it does, it skips the intro.

using UnityEngine;
using System.Collections;

public class ReturnToMenu : MonoBehaviour 
{
    public void Start()
    {
        Application.LoadLevelAdditive("Menu");
    }
}
