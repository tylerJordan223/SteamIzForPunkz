using GInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlotScript : MonoBehaviour
{
    //script to handle everything related to the slot machine//
    
    //info for the spinning of the reels
    [SerializeField] List<GameObject> reels = new List<GameObject>();
    public float spin_speed;

    //input
    private bool spinning;
    private GameInput input;

    private void Start()
    {
        spinning = false;
    }

    private void OnEnable()
    {
        //enable necessary input
        input = new GameInput();
        input.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        //disable necessary input
        input.Player.Interact.Disable();
    }

    private void Update()
    {
        if(input.Player.Interact.IsPressed() && !spinning)
        {
            spinning = true;
            BeginRoll();
        }
    }

    //function to initialize the coroutines that spin the slots
    private void BeginRoll()
    {
        //for each reel run the coroutine to spin it
        for(int i = 0; i < reels.Count; i++)
        {
            StartCoroutine(SpinReel(reels[i].GetComponent<RectTransform>(), i));
        }
    }

    private IEnumerator SpinReel(RectTransform r, int delay)
    {
        //wait based on delay
        yield return new WaitForSeconds(delay);

        //starting velocity
        float velocity = 0.1f;
        float timer = 0.0f;

        while(timer < 5f)
        {
            Vector3 pos = r.localPosition;
            pos.y -= velocity * spin_speed * Time.deltaTime;
            if(pos.y < -16)
            {
                pos.y = 16 - (pos.y + 16);
            }

            r.localPosition = pos;
            
            //increase velocity overtime
            if(velocity < 1)
            {
                velocity += 0.001f;
            }

            //increase the timer and wait until end of frame
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //slowdown until slow enough to do this
        while (true)
        {
            Vector3 pos = r.localPosition;
            pos.y -= velocity * spin_speed * Time.deltaTime;
            if (pos.y < -16)
            {
                //check for end of loop
                if(velocity <= 0.1f)
                {
                    //align properly and break
                    pos.y = -16;
                    break;
                }

                pos.y = 16 - (pos.y + 16);
            }

            r.localPosition = pos;

            //decrease velocity overtime
            if (velocity > 0.1f)
            {
                velocity -= 0.001f;
            }

            yield return new WaitForEndOfFrame();
        }

        //if its the last wheel stopping then no longer spinning
        if(delay == reels.Count-1)
        {
            //RUN CODE TO CALCULATE ITEM OUTPUT
            spinning = false;
        }

        yield return null;
    }
}
