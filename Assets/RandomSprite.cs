using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField]
    private Sprite[] randomSprites;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = randomSprites[Random.Range(0, randomSprites.Length)];
    }
}
