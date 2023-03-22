using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Vector3 location1, location2;
    private float randomMod;
    private float cPingPong = 0;
    private float pPingPong = 0;

    public float movementAnimationLength = 1;

    private float spriteScale = 1;
    private int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = location1;
        randomMod = UnityEngine.Random.Range(0, 100) / 100f;
        cPingPong = randomMod;

        spriteScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        cPingPong = Mathf.PingPong(Time.time / movementAnimationLength, 1);

        // going Left
        if (cPingPong < pPingPong) direction = -1;
        // going right
        else direction = 1;

        float newPos = Vector3.Lerp(location1, location2, cPingPong).x;

        transform.position = new Vector3(newPos, transform.position.y, transform.position.z);
        transform.localScale = new Vector3(direction * spriteScale, transform.localScale.y, transform.localScale.z);

        pPingPong = cPingPong;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.transform.tag)
        {
            case "Player":
                break;
            case "Coin":
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider);
                break;
            default:
                break;
        }
    }

}
