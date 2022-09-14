using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public ParticleSystem coffeeParticle;
    public ParticleSystem timeParticle;
    public ParticleSystem revertParticle;
    public GameObject ice;
    public GameObject barrier;

    [SerializeField] float moveSpeed;

    private float horizontal;

    [SerializeField] bool rightBtnActive;
    [SerializeField] bool leftBtnActive;
    public bool isInvincibility;
    [SerializeField] float maxInvincibileCool;
    [SerializeField] bool isFreezing;
    private float curinvincivileCool;
    private float curFreezeCool;

    [SerializeField] float rightBorder;
    [SerializeField] float leftBorder;

    [SerializeField]
    private GameObject smellScreenOb;
    [SerializeField]
    private GameObject smellScreenOb2;
    private void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
 
        SetInvincibility(true);
    }

    private void Update()
    {
        if (gameManager.testMode)
        {
            if (Input.GetKey(KeyCode.RightArrow))
                horizontal = 1;
            else if (Input.GetKey(KeyCode.LeftArrow))
                horizontal = -1;
            else
                horizontal = 0;
        }
        else
        {
            if (rightBtnActive)
                horizontal = 1;
            else if (leftBtnActive)
                horizontal = -1;
            else
                horizontal = 0;
        }
        PlayerMove();
        ScreenChk();
        if (isInvincibility)
        {
            curinvincivileCool -= Time.deltaTime;
            if (curinvincivileCool < 0)
            {
                curinvincivileCool = maxInvincibileCool;
                SetInvincibility(false);
            }
        }
        if (isFreezing)
        {
            curFreezeCool -= Time.deltaTime;
            if (curFreezeCool < 0)
            {

                Freeze(false);
            }
        }
        animator.SetInteger("Horizontal", (int)horizontal);
    }

    private void PlayerMove()
    {
        int reverseIndex = gameManager.isReverse ? -1 : 1;
        int freezeIndex = isFreezing ? 0 : 1;
        transform.position += new Vector3(moveSpeed * horizontal * freezeIndex * reverseIndex, 0);
    }

    public void RightBtn(bool isRight)
    {
        if (isRight)
            rightBtnActive = !rightBtnActive;
        else
            leftBtnActive = !leftBtnActive;
    }



    private void ScreenChk()
    {
        Vector3 worldPos = Camera.main.WorldToViewportPoint(this.transform.position);
        if (worldPos.x < leftBorder) worldPos.x = leftBorder;
        if (worldPos.x > rightBorder) worldPos.x = rightBorder;
        transform.position = Camera.main.ViewportToWorldPoint(worldPos);
    }

    private int chickenStack;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Freeze")
        {
            curFreezeCool = 0.5f;
            Freeze(true);
        }
        if (collision.tag == "Smell")
        {
            Instantiate(smellScreenOb, new Vector3(0, 0), Quaternion.identity);
        }
        if (collision.tag == "Smell2")
        {
            Instantiate(smellScreenOb2, new Vector3(0, 0), Quaternion.identity);
        }
        if (isInvincibility) return;
        if(collision.tag == "Damage")
        {
            gameManager.GetDamaged();
            SetInvincibility(true);
            SoundManager.instance.Play("Hit");
        }
        if (collision.tag == "Card")
        {
            gameManager.FeverTimeStart();
            SoundManager.instance.Play("CardGet");
        }
        if (collision.tag == "TimeSlow")
        {
            gameManager.CoffeeTimeStart();
        }
        if (collision.tag == "TimeFast")
        {
            gameManager.TimeFasterStart();
        }
        if (collision.tag == "Reverce")
        {
            gameManager.ReverceTimeStart();
        }
        if (collision.tag == "Virus")
        {
            gameManager.HitVirus();
        }
        if (collision.tag == "CreditCard")
        {
            gameManager.AddMoneyValue(-1000000);
        }
        if (collision.tag == "Chicken")
        {
            chickenStack++;
            if (chickenStack > 25)
            {
                gameManager.AddHpValue(1, true);
                chickenStack = 0;
            }

        }
    }
    public void SetInvincibility(bool active)
    {
        isInvincibility = active;
        barrier.SetActive(false);
        spriteRenderer.color = new Color(1, 1, 1, active ? 0.5f : 1);
    }
    public void Freeze(bool active)
    {
        isFreezing = active;
        ice.SetActive(active);
        spriteRenderer.color = new Color(active ? 0.345f : 1, active ? 0.956f : 1, 1, spriteRenderer.color.a);
        animator.speed = active ? 0 : 2;
    }
}
