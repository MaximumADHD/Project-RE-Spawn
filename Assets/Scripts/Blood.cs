using UnityEngine;
using System.Collections;

public class Blood : MonoBehaviour 
{
    public float velocity = 250;
    public float lifeCycle = 5;
    private float killAt;

    public void Start()
    {
        rigidbody2D.angularVelocity = Random.Range(-360f, 360f);
        rigidbody2D.AddForce(new Vector2(Random.Range(-velocity,velocity),Random.Range(-velocity,velocity)));
        Object.DestroyObject(this.gameObject, lifeCycle);
    }
}
