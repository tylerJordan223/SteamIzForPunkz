using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    //all variables for the parts of the model
    private Transform trans;
    private Rigidbody2D rb;
    private Collider2D coll;

    //graphic stuff
    private Animator anim;
    private SpriteRenderer sr;

    //character mechanics
    private float playerSpeed;

    //Stats
    [Header("Stats")]
    private PlayerStats pstats;

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
    public GameObject currentCamera;

    //movement
    public bool canControl;
    Vector2 inputVector;
    Vector2 lastInputVector;

    //damage
    [Header("Damage Timer")]
    [SerializeField] public float timeBetweenDamage;
    private float damageTimer;

    [Header("Animation")]
    [SerializeField] Animator head_anim;
    [SerializeField] Animator body_anim;

    private void Start()
    {
        //object variables
        trans = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        //graphics
        anim = GetComponent<Animator>();

        //character mechanics
        playerSpeed = 5f;

        //getting statistics
        pstats = GetComponent<PlayerStats>();

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
        currentCamera = null;

        //flags
        canControl = false;
        holdingItem = false;

        //damage timer
        damageTimer = timeBetweenDamage;

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
                    heldObject.transform.position = new Vector3(heldObject.transform.position.x, heldObject.transform.position.y - (coll.bounds.size.y / 2), heldObject.transform.position.z);
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
            if (Input.GetKeyDown(KeyCode.F) && canDash && !(inputVector.x == 0f && inputVector.y == 0f))
            {
                StartCoroutine(Dash());
            }
        }
        else
        {
            //if can't control then stop
            inputVector = new Vector2(0f, 0f);
        }

        //updates damage visually
        damageTimer += Time.deltaTime;

        //checks for when the player is dashing
        if (dashing)
        {
            //cancel control etc
        }

        //kill when out of hp
        if (pstats.health <= 0)
        {
            canControl = false;
            pstats.health = 0;

            StartCoroutine(Death());
        }

        //update Animation
        UpdateAnimation();

        //updates to do only when the game is running
        if(DataManager.playing)
        {
            //update the player node in the grid
            UpdatePlayerNode();
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            DataManager.g.checkGrid();
        }

    }


    //use for anything movement related
    private void FixedUpdate()
    {
        //normalize for the sake of diagonal not double speed
        inputVector.Normalize();

        //update on speed if alive, stop if else
        if(pstats.health > 0)
        {
            rb.velocity = inputVector * playerSpeed * pstats.dynSpeed;
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    public void Damage(float d)
    {
        if (damageTimer > timeBetweenDamage)
        {
            //reset the timer
            damageTimer = 0f;
            pstats.health -= 1;
        }
    }

    //handle the death sequence
    IEnumerator Death()
    {
        inputVector = new Vector2(0f, 0f);
        head_anim.SetBool("dead", true);
        yield return new WaitForSeconds(4f);
        GameObject.Find("GameOver").GetComponent<Animator>().SetBool("over", true);
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
        Vector2 anim_vector = lastInputVector.normalized;

        //setting the values necessary
        head_anim.SetFloat("x", anim_vector.x);
        body_anim.SetFloat("x", anim_vector.x);
        head_anim.SetFloat("y", anim_vector.y);
        body_anim.SetFloat("y", anim_vector.y);

        //updating for idle
        if(inputVector.x == 0f && inputVector.y == 0f)
        {
            //make sure the animation speed of the body is 0 if this is the case
            body_anim.SetBool("idle", true);
        }
        else
        {
            body_anim.SetBool("idle", false);
            //scaling the animation speed with the player speed
            body_anim.speed = 1 + pstats.dynSpeed;
        }

        //updating for damage
        if((damageTimer < timeBetweenDamage) && (pstats.health > 0))
        {
            head_anim.SetBool("hurt", true);
        }
        else
        {
            head_anim.SetBool("hurt", false);
        }
    }

    private void UpdatePlayerNode()
    {
        if(DataManager.g.getNode(trans) != null)
        {
            DataManager.g.setPlayerNodeAtNode(DataManager.g.getNode(trans));
        }

        //update and close doors if entering a room wtih enemies
    }

    //to handle collisions
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Room")
        {
            if(currentRoom == null)
            {
                //actually returns the room based on the floor
                currentRoom = collision.transform.parent.parent.gameObject;
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
