// Max G 2015 <3
// Monster.cs

using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour 
{
    public GameObject LeftNode;
    public GameObject RightNode;
    public float MoveSpeed = 2;
    private float currentForce = -1;

    private void setLocalScale(float x = 0.1f, float y = 0.1f, float z = 0.1f)
    {
        transform.localScale = new Vector3(x, y, z);
    }
	
	public void Update() 
    {
        // EXTREMELY BASIC AI
        // We don't have enough time to make these guys smart
        if (currentForce == -1)
        {
            if (transform.localPosition.x <= LeftNode.transform.localPosition.x)
            {
                currentForce = 1;
            }
        }
        else if (currentForce == 1)
        {
            if (transform.localPosition.x >= RightNode.transform.localPosition.x)
            {
                currentForce = -1;
            }
        }
        setLocalScale(currentForce/10);
        rigidbody2D.velocity = new Vector2(currentForce*MoveSpeed,rigidbody2D.velocity.y);
	}

    public void OnCollisionEnter2D(Collision2D collider)
    {
        GameObject hit = collider.gameObject;
        if (hit.tag == "Player")
        {
            Player player = GameObject.FindObjectOfType<Player>();
            player.Dead = true;
        }
    }
}