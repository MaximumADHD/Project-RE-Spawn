using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour 
{
    public bool triggered = false;
    public Invokable targetObject;

    public void Start()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 1, 1, 0);
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.tag == "Player" && !triggered)
        {
            Debug.Log("Trigger Hit");
            triggered = true;
            targetObject.SetActive(true);
        }
    }
}
