using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Animations;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;

    public float speed;
    public float jumpForce;

    public Text scoreText;
    public Text livesText;
    public Text youWinText;

    private int scoreValue = 0;
    private int coinCount;
    private int lives;
    private const int MAX_LIVES = 3;
    private const float MAX_SPEED = 6f;
    private bool controlsDisabled = false;

    private int currentLevel = 1;
    private int levelCount;

    private float playerScale;

    public Vector2[] levelLocations;

    public AudioSource musicSource, sfxSource;
    public AudioClip coinCollected, gameWon, gameLost, levelWon;

    public Animator animator;
    private bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();

        levelCount = levelLocations.Length;

        youWinText.enabled = false;

        playerScale = transform.localScale.x;

        SetScore(0);
        MoveToLevel();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        float horiMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");

        if (!controlsDisabled)
            rd2d.AddForce(new Vector2(horiMovement * speed, vertMovement * (speed / 2)));

        if (rd2d.velocity.x < 0) transform.localScale = new Vector3(-playerScale, transform.localScale.y, transform.localScale.z);
        else if (rd2d.velocity.x > 0) transform.localScale = new Vector3(playerScale, transform.localScale.y, transform.localScale.z);

        SpeedCheck();

        AnimationCheck();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log($"COLLDIED WITH: {collision.gameObject}");


        switch (collision.transform.tag)
        {

            case "Ground":
                onGround = true;
                break;

            case "Coin":
                SetScore(scoreValue + 1);
                sfxSource.clip = coinCollected;
                sfxSource.Play();
                Destroy(collision.collider.gameObject);

                if (scoreValue >= coinCount)
                    YouWin();
                break;

            case "Enemy":
                SetLives(lives - 1);
                MoveToLevelNoReset();
                Destroy(collision.gameObject);
                rd2d.velocity = Vector2.zero;

                if (lives <= 0)
                    YouLose();
                break;

            default:
                break;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.transform.tag)
        {
            case "Ground":
                onGround = false;
                break;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var myCollider = GetComponent<Collider2D>();
        float bottom = myCollider.bounds.center.y - myCollider.bounds.extents.y;

        if (collision.collider.tag == "Ground" && bottom > collision.contacts[0].point.y)
        {
            // onGround = true;

            if (Input.GetKey(KeyCode.W) && !controlsDisabled)
                rd2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        else
        {
            // onGround = false;
        }
    }

    private void SpeedCheck()
    {
        if (rd2d.velocity.x < -MAX_SPEED)
            rd2d.velocity = new Vector2(-MAX_SPEED, rd2d.velocity.y);
        else if (rd2d.velocity.x > MAX_SPEED)
            rd2d.velocity = new Vector2(MAX_SPEED, rd2d.velocity.y);
    }

    private void SetScore(int amt)
    {
        scoreValue = amt;
        scoreText.text = $"Score: {scoreValue} / {coinCount}";
    }

    private void SetLives(int amt)
    {
        lives = amt;
        livesText.text = $"Lives: {lives}";

    }

    private void YouWin()
    {
        currentLevel += 1;

        if (currentLevel > levelCount)
        {
            youWinText.enabled = true;
            controlsDisabled = true;

            musicSource.Stop();
            musicSource.clip = gameWon;
            musicSource.Play();

        }
        else
        {
            MoveToLevel();
        }
    }

    private void YouLose()
    {
        controlsDisabled = true;
        youWinText.text = $"You Lose!\nGame by Dimitri Palmer";
        youWinText.enabled = true;

        musicSource.Stop();
        musicSource.clip = gameLost;
        musicSource.Play();

    }

    private int GetLevelCoinCount()
    {
        GameObject levelObject = GameObject.Find($"Level {currentLevel}");
        int validChildCount = levelObject.transform.Find("Coins").transform.childCount;

        // Debug.Log($"COIN COUNT FOR LEVEL {currentLevel}: {validChildCount}");
        return validChildCount;
    }

    private void MoveToLevel()
    {
        MoveToLevelNoReset();

        SetLives(MAX_LIVES);

        SetScore(0);
        coinCount = GetLevelCoinCount();

        MoveToLevelNoReset();
    }

    private void MoveToLevelNoReset()
    {
        Vector2 cLocation = levelLocations[currentLevel - 1];
        transform.position = cLocation;
    }

    private void AnimationCheck()
    {
        var collider = GetComponent<Collider2D>();

        // var groundObjects = GameObject.FindWithTag("Ground");
        // var cF = new ContactFilter2D();

        // Collider2D[] res = new Collider2D[1];


        if (!onGround) animator.SetInteger("State", 2);
        else
        {
            if (Math.Abs(rd2d.velocity.x) >= 0.5f) animator.SetInteger("State", 1);
            else animator.SetInteger("State", 0);
        }
    }

}