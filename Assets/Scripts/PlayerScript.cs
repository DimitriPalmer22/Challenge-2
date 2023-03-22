using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;

    public float speed;
    public float jumpForce;

    public Text score;
    public Text youWinText;

    private int scoreValue = 0;
    private int coinCount;
    private int lives;
    private const int MAX_LIVES = 3;

    private const float MAX_SPEED = 5f;
    private bool gameWon = false;

    private int currentLevel = 1;
    private int levelCount;

    public Vector2[] levelLocations;

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();

        levelCount = levelLocations.Length;
        coinCount = GetLevelCoinCount();
        SetScoreText();

        youWinText.enabled = false;

        lives = MAX_LIVES;

        MoveToLevel();
    }

    void FixedUpdate()
    {
        float horiMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");

        if (!gameWon)
            rd2d.AddForce(new Vector2(horiMovement * speed, vertMovement * (speed / 2)));

        SpeedCheck();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            SetScoreText();
            Destroy(collision.collider.gameObject);

            if (scoreValue >= coinCount)
                YouWin();

        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        var myCollider = GetComponent<Collider2D>();
        float bottom = myCollider.bounds.center.y - myCollider.bounds.extents.y;

        // Debug.Log($"Collision: {bottom} vs {collision.contacts[0].point.y}");

        if (collision.collider.tag == "Ground" && bottom > collision.contacts[0].point.y)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }
        }
    }

    private void SpeedCheck()
    {
        if (rd2d.velocity.x < -MAX_SPEED)
            rd2d.velocity = new Vector2(-MAX_SPEED, rd2d.velocity.y);
        else if (rd2d.velocity.x > MAX_SPEED)
            rd2d.velocity = new Vector2(MAX_SPEED, rd2d.velocity.y);
    }

    private void SetScoreText()
    {
        score.text = $"Score: {scoreValue} / {coinCount}";
    }

    private void YouWin()
    {
        currentLevel += 1;

        if (currentLevel > levelCount)
        {
            youWinText.enabled = true;
            gameWon = true;
        }

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
        Vector2 cLocation = levelLocations[currentLevel - 1];
        transform.position = cLocation;
    }

}