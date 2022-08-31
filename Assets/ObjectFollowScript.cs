using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowScript : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        transform.position = new Vector2(transform.position.x, 0);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, 0,0) , new Vector3(target.position.x, 0, 0), 0.03f);
    }
}
