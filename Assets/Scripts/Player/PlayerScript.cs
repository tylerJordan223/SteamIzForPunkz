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
    public float health;
    public float charges;
    private float playerSpeed;

    //Dynamic Attributes
    [Header("Stats")]
    public float dynSpeed;
    public float dynDashSpeed;
    public float dynMeleeDamage;
    public float dynRangeDamage;
    public float dynMaxCharges;
    public float maxHealth;
    public float luck;

    [Header("Debug")]
    public bool debug;

    //dash variables
    private bool canDash;
    private bool dashing;
    private float dashDistance;
    private float dashTime;
    private float dashCooldown;

    [Header("Dash")]
    [SerializeField] private TrailRenderer dashTrail;

    //Saved Objects
    [Header("Inventory")]
    public GameObject heldObject;
    public GameObject currentRoom;
    public List<GameObject> itemList = new List<GameObject>();
    public bool holdingItem;
    public float moneyCount;

    //movement
    private bool canControl;
    Vector2 inputVector;
    Vector2 lastInputVector;

    //decomposed world
    private Grid g;

    //DEBUG STAT CHANGER
    List<float> stats;
    public int selected_stat;

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
        health = 3f;
        charges = 3f;
        playerSpeed = 5f;

        //dynamic mechanics
        dynSpeed = 1f;
        dynDashSpeed = 1f;
        dynMeleeDamage = 1f;
        dynRangeDamage = 1f;
        dynMaxCharges = 3f;
        maxHealth = 3f;
        luck = 1f;

        //DEBUG STAT CHANGER
        debug = false;
        stats = new List<float>();
        stats.Add(dynSpeed);
        stats.Add(dynDashSpeed);
        stats.Add(dynMeleeDamage);
        stats.Add(dynRangeDamage);
        stats.Add(dynMaxCharges);
        stats.Add(charges);
        stats.Add(maxHealth);
        stats.Add(health);
        stats.Add(luck);
        selected_stat = 0;

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
        moneyCount = 0;

        //flags
        canControl = true;
        holdingItem = false;

        //getting the grid from the floor generator
        GameObject f = GameObject.Find("FloorGenerator");
        if(f != null)
        {
            g = f.GetComponent<FloorGenerator>().g;
        }
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



        //debug stuff

        //activates debug stats
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            debug = !debug;
        }

        if (debug)
        {
            //RESTARTS GAME!!!
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }


            //DEBUG STAT CHANGER
            //set all values before changing
            stats[0] = dynSpeed;
            stats[1] = dynDashSpeed;
            stats[2] = dynMeleeDamage;
            stats[4] = dynMaxCharges;
            stats[5] = charges;
            stats[6] = maxHealth;
            stats[7] = health;
            stats[8] = luck;

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                if(selected_stat != stats.Count-1)
                {
                    selected_stat++;
                }
                else
                {
                    selected_stat = 0;
                }
            } 
            if(Input.GetKeyDown(KeyCode.Keypad8))
            {
                if (selected_stat != 0)
                {
                    selected_stat--;
                }
                else
                {
                    selected_stat = stats.Count;
                }
            } 
            if(Input.GetKeyDown(KeyCode.Keypad4))
            {
                if (stats[selected_stat] > 0)
                {
                    stats[selected_stat]--;
                }
            } 
            if(Input.GetKeyDown(KeyCode.Keypad6))
            {
                stats[selected_stat]++;
            }

            //set all values if changed
            dynSpeed = stats[0];
            dynDashSpeed = stats[1];
            dynMeleeDamage = stats[2];
            dynRangeDamage = stats[3];
            dynMaxCharges = stats[4];
            charges = stats[5];
            maxHealth = stats[6];
            health = stats[7];
            luck = stats[8];
            //DEBUG STAT CHANGER
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
            rb.velocity = inputVector * playerSpeed * dynSpeed;
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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyLayer"), LayerMask.NameToLayer("PlayerLayer"), true);
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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("EnemyLayer"), LayerMask.NameToLayer("PlayerLayer"), false);
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
        WorldNode startPos = g.nodes[Random.Range(0, g.gridLength), Random.Range(0, g.gridHeight)];
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }

}
