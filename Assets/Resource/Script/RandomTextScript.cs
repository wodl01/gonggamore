using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RandomText
{
    public string randomText;
    public float boxColSize;
}

public class RandomTextScript : MonoBehaviour
{
    [SerializeField] TextMesh textMesh;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] List<RandomText> randomText;


    void Start()
    {
        RandomText curTextInfo = randomText[Random.Range(0, randomText.Count)];
        textMesh.text = curTextInfo.randomText;
        boxCollider.size = new Vector2(curTextInfo.boxColSize, boxCollider.size.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
