using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    //all the things included in the item UI
    public Image itemImage;
    public TextMeshProUGUI item_name;
    public TextMeshProUGUI item_description;
    public TextMeshProUGUI item_stats_up;
    public TextMeshProUGUI item_stats_down;

    //private values for the script
    private Animator anim;
    private Queue<ItemScript> item_queue;
    private bool flag;

    private void Start()
    {
        anim = GetComponent<Animator>();
        item_queue = new Queue<ItemScript>();
        flag = false;
    }

    private void Update()
    {
        //this is only to handle overflow of items
        if(flag && item_queue.Count > 0)
        {
            StartCoroutine(show_item(item_queue.Dequeue()));
        }
    }

    public IEnumerator show_item(ItemScript i)
    {
        if(!anim.GetBool("active"))
        {
            itemImage.sprite = i.isprite;
            item_name.text = i.item_name;
            item_name.color = i.glow_color;
            item_description.text = i.description;
            item_stats_up.text = i.stats_up;
            item_stats_down.text = i.stats_down;

            anim.SetBool("active", true);
            flag = false;
            yield return new WaitForSeconds(5f);
            anim.SetBool("active", false);
            yield return new WaitForSeconds(1.5f);
            flag = true;
        }
        else
        {
            item_queue.Enqueue(i);
        }
    }
}
