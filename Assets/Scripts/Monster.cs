// Max G 2015 <3
// Monster.cs

using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour 
{
    public SpriteSheet GhostAnim;
    public float MoveSpeed = 2;

    private float currentForce = -1;
    private SpriteRenderer sprite;

    private void setLocalScale(float x = 1f, float y = 1f, float z = 1f)
    {
        transform.localScale = new Vector3(x, y, z);
    }

    private bool isEditorMode()
    {
        GameObject editorObj = GameObject.FindGameObjectWithTag("LevelEdit");
        return editorObj != null;
    }

    public void Start()
    {
        if (!isEditorMode())
        {
            Rigidbody2D r = gameObject.AddComponent<Rigidbody2D>();
            r.fixedAngle = true;
        }
        sprite = GetComponent<SpriteRenderer>();
    }
	
	public void Update() 
    {
        // EXTREMELY BASIC AI
        // We don't have enough time to make these guys smart
        if (!isEditorMode())
        {
            Vector2 myPos = new Vector2(transform.localPosition.x, transform.localPosition.y);
            if (currentForce == -1)
            {
                Vector2 castFrom = myPos + new Vector2(transform.localScale.x / -2, 0);
                RaycastHit2D ray = Physics2D.Raycast(castFrom, castFrom + new Vector2(-500, 0));
                if ((myPos - ray.centroid).magnitude < 0.8)
                {
                    currentForce = 1;
                }
            }
            else if (currentForce == 1)
            {
                Vector2 castFrom = myPos + new Vector2(transform.localScale.x / 2, 0);
                RaycastHit2D ray = Physics2D.Raycast(castFrom, castFrom + new Vector2(500, 0));
                if ((myPos - ray.centroid).magnitude < 1)
                {
                    currentForce = -1;
                }
            }
            setLocalScale(-currentForce);
            rigidbody2D.velocity = new Vector2(currentForce * MoveSpeed, rigidbody2D.velocity.y);
            try
            {
                Sprite next = GhostAnim.NextFrame();
                if (next != null)
                {
                    sprite.sprite = next;
                }
            }
            catch
            {
                // *awkward whistle*
            }
        }
	}

    public void OnCollisionEnter2D(Collision2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.tag == "Player")
        {
            Player player = GameObject.FindObjectOfType<Player>();
            player.Kill("Ghosts are very spooky.");
        }
    }
}