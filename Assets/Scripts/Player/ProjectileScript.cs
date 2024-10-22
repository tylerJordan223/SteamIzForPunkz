using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int maxRicochets;
    private int ricochetCount;
    private float bulletSpeed;
    private Rigidbody2D rb;
    private Transform trans;
    private Vector3 lastVelocity;

    private void Start()
    {
        ricochetCount = maxRicochets;
        bulletSpeed = 1f;
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();

        //rb.AddForce(trans.up * 500, ForceMode2D.Force);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;

        if (ricochetCount < 0)
        {
            //Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(ricochetCount > 0)
        {
            //raycast to hit points of damage
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(trans.position.x, trans.position.y), new Vector2(trans.up.x, trans.up.y), Mathf.Infinity, LayerMask.GetMask("ObstacleLayer")); ;

            //if it hits
            if(hit)
            {
                //get the direction of the point to the object
                Vector2 dir = (hit.transform.position - trans.position);

                //if it hits a wall
                if(hit.collider.gameObject.tag == "wall")
                {

                    //raycast to damage enemies, necessary to hit all
                    //get a list of enemy hits: its necessary to do this after the object moves because otherwise it cant get the right length
                    RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(trans.position.x, trans.position.y), new Vector2(trans.up.x, trans.up.y), hit.distance, LayerMask.GetMask("EnemyLayer"));
                    if (hits != null)
                    {
                        foreach(RaycastHit2D h in hits)
                        {
                            //DAMAGE EACH ENEMY//
                        }
                    }

                    //move the object to the hit point
                    Debug.Log(hit.point);
                    trans.position = hit.point;

                    //rotate properly
                    trans.up = Vector2.Reflect(dir.normalized, hit.normal);
                    Debug.Log(trans.up.ToString());

                    ricochetCount--;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        if(collision.gameObject.CompareTag("wall"))
        {
            var speed = lastVelocity.magnitude;
            var dir = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

            rb.velocity = dir * Mathf.Max(speed, 0f);
            ricochetCount--;
        }
        */
    }
}
