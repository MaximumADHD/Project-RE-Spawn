using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour 
{
    public Sprite bloodySpike;
    public AudioClip hitSound;

    private bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (!isEditorMode())
        {
            GameObject hit = collider.gameObject;
            if (hit.name == "Player")
            {
                if (hit.transform.localPosition.y < transform.localPosition.y + 0.5f)
                {
                    Player player = GameObject.FindObjectOfType<Player>();
                    if (!player.Dead)
                    {
                        foreach (Spikes spike in GameObject.FindObjectsOfType<Spikes>())
                        {
                            GameObject g = spike.gameObject;
                            if ((g.transform.localPosition - transform.localPosition).magnitude <= 1)
                            {
                                SpriteRenderer s = g.GetComponent<SpriteRenderer>();
                                if (s != null)
                                {
                                    s.sprite = bloodySpike;
                                }
                            }
                        }
                        AudioSource.PlayClipAtPoint(hitSound, transform.localPosition, 1);
                        player.Kill("Jumping into a pit of spikes\nprobably isn't a good idea.");
                    }
                }
            }
        }
    }
}
