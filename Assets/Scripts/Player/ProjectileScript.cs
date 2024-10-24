using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int maxRicochets;
    public float projectileDamage;
    private int ricochetCount;
    private float bulletSpeed;
    private Transform trans;

    private void Start()
    {
        ricochetCount = maxRicochets;
        bulletSpeed = 1f;
        projectileDamage = 5f;
        trans = GetComponent<Transform>();
    }

    private void Update()
    {
        if (ricochetCount == 0)
        {
            Destroy(this.gameObject, 5f);
        }
    }

    private void FixedUpdate()
    {
        if(ricochetCount != 0)
        {
            //raycast to hit points of damage
            RaycastHit2D hit = Physics2D.Raycast(trans.position + trans.up.normalized/10, trans.up, Mathf.Infinity, LayerMask.GetMask("ObstacleLayer")); ;
            //multiplied by infinity because that value is not just direction its also distance
            Debug.DrawRay(trans.position, trans.up.normalized * 100f, Color.blue, 100f);

            //if it hits
            if(hit)
            {
                //get the direction of the point to the object
                Vector2 dir = ((Vector3)hit.point - trans.position);

                //if it hits a wall
                if(hit.collider.gameObject.tag == "wall" || hit.collider.gameObject.tag == "Door")
                {

                    //raycast to damage enemies, necessary to hit all
                    //get a list of enemy hits: its necessary to do this after the object moves because otherwise it cant get the right length
                    RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(trans.position.x, trans.position.y), new Vector2(trans.up.x, trans.up.y), hit.distance, LayerMask.GetMask("EnemyLayer"));
                    if (hits != null)
                    {
                        foreach(RaycastHit2D h in hits)
                        {
                            EnemyHealth e = h.collider.gameObject.GetComponent<EnemyHealth>();
                            e.Damage(projectileDamage);
                        }
                    }

                    //move the object to the hit point
                    Debug.Log(hit.point);
                    trans.position = hit.point;

                    //rotate properly
                    trans.up = Vector2.Reflect(dir.normalized, hit.normal);

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
