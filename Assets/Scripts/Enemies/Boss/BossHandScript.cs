using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandScript : MonoBehaviour
{
    [SerializeField] GameObject boss_head;

    private float head_offset;
    private bool idle;

    private void Start()
    {
        head_offset = transform.position.x - boss_head.transform.position.x;
        idle = true;
    }

    private void Update()
    {
        if(idle)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(boss_head.transform.position.x + head_offset, transform.position.y, 0f), Time.deltaTime);
        }
    }
}
