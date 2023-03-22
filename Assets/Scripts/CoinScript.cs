using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        switch (collision.transform.tag)
        {
            case "Player":
                // Debug.Log("PLAYER");
                // audioSource.Play();
                break;
            default:
                break;
        }

    }

}
