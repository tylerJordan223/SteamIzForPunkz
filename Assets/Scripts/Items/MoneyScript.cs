using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    /*
        Explanation:

        When a brass object is spawned:
        *determines how much the brass is worth (copper = 1, brass = 5)
        *picks a spot within 1 space
        *moves to that distance
        *after it settles if the player is within its outer collider it moves to the the player
        *is picked up when entering the player collider
     */

    //basic variables
    private PlayerScript player;
    private bool pickupable;
    private bool to_player;
    public float worth;

    //random factors
    private float r_disx;
    private float r_disy;

    //transforms for movement
    private Transform t;
    private Vector3 destination;
    private float lerpSpeed = 1f;
    private Rigidbody2D rb;

    private void Start()
    {
        //not pickupable initially
        pickupable = false;
        rb = GetComponent<Rigidbody2D>();

        //just being safe in case it can't find player, but if theres money there should be a player
        if(GameObject.Find("Tric") != null)
        {
            player = GameObject.Find("Tric").GetComponent<PlayerScript>();
        }

        //determine the cost
        float r = Random.Range(1f, 10f);
        //20% chance to be worth 5 instead of 1 (Brass instead of Copper)
        if (r < 8) { worth = 1f; } else { worth = 5f; };

        //get the random location's x and y, and also making sure its at least 0.05
        r_disx = Random.Range(-1f, 1f);
        r_disx += Mathf.Sign(r_disx) * 0.5f;
        r_disy = Random.Range(-1f, 1f);
        r_disy += Mathf.Sign(r_disy) * 0.5f;

        //get transforms
        t = GetComponent<Transform>();
        destination = t.position + new Vector3(r_disx, r_disy, 0f);

        //function waits 3 seconds then makes the money move towards player
        StartCoroutine(MovementCountdown());
    }

    private void FixedUpdate()
    {
        if(!pickupable)
        {
            //lerp between current location, and current location + distance
            Vector3 iPosition = Vector3.Lerp(t.position, destination, Time.deltaTime * lerpSpeed);
            t.position = iPosition;
        }

        if(to_player)
        {
            //repeat from other code just with the player instead
            Vector3 iPosition = Vector3.Lerp(t.position, player.transform.position, Time.deltaTime * lerpSpeed * 1.5f);
            t.position = iPosition;
        }

        rb.velocity = new Vector3(0f,0f,0f);
    }

    //function to wait before the money goes towards player
    private IEnumerator MovementCountdown()
    {
        yield return new WaitForSeconds(3f);
        pickupable = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //when a player is within the field
        if(pickupable && other.gameObject.tag == "Player")
        {
            to_player = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //when the player picks up the money
        if(collision.gameObject.tag == "Player")
        {
            //give player money and destroy
            player.gameObject.GetComponent<PlayerStats>().moneyCount += worth;
            //INSERT AUDIO FOR COIN PICKUP HERE
            Destroy(this.gameObject);
        }
    }
}
