using UnityEngine;
using System.Collections;

public class LavaScript : MonoBehaviour 
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.name == "Player")
        {
            Player player = GameObject.FindObjectOfType<Player>();
            player.Kill("Jumping into a pit of spikes\nprobably isn't a good idea.");
        }
    }
}
