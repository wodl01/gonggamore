using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMoving : MonoBehaviour
{
    [SerializeField] float rightBorder;
    [SerializeField] float leftBorder;

    bool sightGoRight;
    bool sightOn;
    float sightSpeed;
    // Start is called before the first frame update
    void Start()
    {
        sightSpeed = Random.Range(-0.02f, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(sightGoRight ? sightSpeed : -sightSpeed, 0, 0));
        Vector3 worldPos = Camera.main.WorldToViewportPoint(transform.position);
        if (worldPos.x < leftBorder) worldPos.x = leftBorder;
        if (worldPos.x > rightBorder) worldPos.x = rightBorder;
        transform.position = Camera.main.ViewportToWorldPoint(worldPos);
    }
}
