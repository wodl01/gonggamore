using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopScript : MonoBehaviour
{
    public GameManager gameManager;

    public Vector2 fallRandomRange;
    [SerializeField] private float fallSpeed;
    [SerializeField] private int moneyValue;
    [SerializeField] private int scoreValue;
    [SerializeField] private bool isBeAffectedTime;
    [SerializeField] private bool isBossPattern;
    // Start is called before the first frame update

    float fallSpeedPer;

    [SerializeField] private bool particleOn;
    [SerializeField] private GameObject[] particles;
    public void SummonEvent()
    {
        fallSpeed = Random.Range(fallRandomRange.x, fallRandomRange.y);
    }


    // Update is called once per frame
    void Update()
    {
        if (isBeAffectedTime)
        {
            if (gameManager.isCoffeeTime)
                fallSpeedPer = 0.5f;
            else if (gameManager.isFastTime)
                fallSpeedPer = 2f;
            else if (gameManager.isFastTime && gameManager.isCoffeeTime)
                fallSpeedPer = 1;
            else
                fallSpeedPer = 1;
        }
        else
            fallSpeedPer = 1;

        if(!gameManager.isPauseGame)
        transform.position += new Vector3(0, fallSpeed * fallSpeedPer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            if (!isBossPattern)
            {
                gameManager.RemovePrefapList(gameObject);
                gameManager.AddScoreValue(scoreValue);
            }
            if(particleOn)
                for (int i = 0; i < particles.Length; i++)
                {
                    Instantiate(particles[i], transform.position, Quaternion.identity);
                }
            SoundManager.instance.Play("Poop");
            Destroy(gameObject);
        }
        if (collision.tag == "Player")
        {
            if (!isBossPattern)
            {
                gameManager.RemovePrefapList(gameObject);
                if (!gameManager.isRepeatMode)
                    gameManager.AddMoneyValue(moneyValue * (gameManager.curStage + 1));
                else
                    gameManager.AddMoneyValue(moneyValue * 10);
                if (moneyValue > 0) SoundManager.instance.Play("CoinGet");
            }

            Destroy(gameObject);
        }
    }
}
