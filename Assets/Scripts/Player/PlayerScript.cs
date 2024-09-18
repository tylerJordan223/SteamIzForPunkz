using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/*

    Purpose: Main Character Control Script
    Author: King_Korby
    Date Created: 9/5/2024

    Legend:
    "//" - Regular Comment
    "//**" - To Be Added
    "*temp*" - Temporary line

*/

public class PlayerScript : MonoBehaviour
{
    //all variables for the parts of the model
    private Transform trans;
    private Rigidbody2D rb;
    private Collider2D collider;

    //graphic stuff
    private Animator anim;
    private SpriteRenderer sr;

    //character mechanics
    private float health;
    private float playerSpeed;

    //dash variables
    private bool canDash;
    private bool dashing;
    private float dashDistance;
    private float dashTime;
    private float dashCooldown;
    [SerializeField] private TrailRenderer dashTrail;

    //Saved Objects
    public GameObject heldObject;
    public GameObject currentRoom;
    public List<GameObject> itemList = new List<GameObject>();

    //flags
    private bool canControl;
    public bool holdingItem;

    //movement
    Vector2 inputVector;
    Vector2 lastInputVector;

    //outside objects
    [SerializeField] WorldDecomp decomposer;

    private void Start()
    {
        //object variables
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();

        //graphics
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        //character mechanics
        health = 100f;
        playerSpeed = 5f;

        //dash mechanics
        dashDistance = 30f;
        dashTime = 1f;
        dashCooldown = 1f;
        dashing = false;
        canDash = true;
        dashTrail.emitting = false;

        //saved objects
        heldObject = null;
        currentRoom = null;

        //flags
        canControl = true;
        holdingItem = false;

        //decompose the world intially
        decomposer.Decompose();

        //move to start position
        //RandomPosition();
    }

    //use for anything non-movement related like math
    private void Update()
    {
        if(canControl)
        {
            //update input
            inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            //update last input if actual input and not 0
            // CURRENTLY SET UP FOR 4 DIRECTIONAL, IF HORIZONTAL THEN USE THAT
            if ( (inputVector.x != 0 || inputVector.y != 0) && (inputVector.x == 0 || inputVector.y == 0) )
            {
                lastInputVector = inputVector;
            }

            //place/pickup item
            if(holdingItem)
            {
                if(Input.GetKeyDown(KeyCode.E))
                {
                    //Setting object's parent to current room
                    heldObject.transform.SetParent(currentRoom.transform.Find("Items").transform, true);
                    heldObject.transform.localScale = Vector3.one;
                    heldObject.transform.position = new Vector3(heldObject.transform.position.x, heldObject.transform.position.y - (collider.bounds.size.y / 2), heldObject.transform.position.z);
                    heldObject = null;
                    holdingItem = false;
                }
            }else
            {
                if(itemList.Count != 0 && Input.GetKeyDown(KeyCode.E))
                {
                    GameObject item = itemList[0];
                    itemList.RemoveAt(0);
                    item.transform.SetParent(trans.Find("HeldItem"), false);
                    item.transform.localPosition = Vector3.zero;
                    heldObject = item;
                    holdingItem = true;
                }
            }

            //dash
            if (Input.GetKeyDown(KeyCode.F) && canDash)
            {
                StartCoroutine(Dash());
            }
        }

        //kill when out of hp
        if (health <= 0)
        {
            canControl = false;
            health = 0;

            StartCoroutine(Death());
        }

        //move to random position
        if(Input.GetKeyDown(KeyCode.P))
        {
            RandomPosition();
        }

        //decompose
        if (Input.GetKeyDown(KeyCode.O))
        {
            decomposer.Decompose();
        }

        //update Animation
        UpdateAnimation();

    }

    //use for anything movement related
    private void FixedUpdate()
    {
        //normalize for the sake of diagonal not double speed
        inputVector.Normalize();

        //update on speed if alive, stop if else
        if(health > 0)
        {
            rb.velocity = new Vector2(inputVector.x * playerSpeed, inputVector.y * playerSpeed);
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    //handle the death sequence
    IEnumerator Death()
    {
        //** set animation to dead
        //** wait for animation to end
        Application.Quit();
        /*temp*/yield return null;
    }

    //handle the dash
    private IEnumerator Dash()
    {
        //begin dash
        canControl = false;
        canDash = false;
        dashing = true;

        //get the direction
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashDistance, 0f);

        //have the trail and dash and disable it
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;

        //cancel dash and enable timer for cooldown
        rb.gravityScale = originalGravity;
        dashing = false;
        canControl = true;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    //Update the Animation
    private void UpdateAnimation()
    {
        //** Update all the animation
    }

    //temporary function for the sake of grid testing
    private void RandomPosition()
    {
        WorldNode startPos = decomposer.nodes[Random.Range(0, decomposer.g.gridLength), Random.Range(0, decomposer.g.gridHeight)];
        trans.position = new Vector3(startPos.x, startPos.y, 0f);
    }

    //to handle collisions
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Room")
        {
            if(currentRoom == null)
            {
                currentRoom = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Room")
        {
            currentRoom = null;
        }
    }

}
