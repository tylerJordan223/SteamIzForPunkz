using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] public List<GameObject> common_items;
    [SerializeField] public List<GameObject> rare_items;
    [SerializeField] public List<GameObject> epic_items;
    [SerializeField] public List<GameObject> legendary_items;

    public List<GameObject> getList()
    {
        List<GameObject> all_items = new List<GameObject>();

        foreach(GameObject i in common_items)
        {
            all_items.Add(i);
        }
        foreach (GameObject i in rare_items)
        {
            all_items.Add(i);
        }
        foreach (GameObject i in epic_items)
        {
            all_items.Add(i);
        }
        foreach (GameObject i in legendary_items)
        {
            all_items.Add(i);
        }

        return all_items;
    }

    public GameObject GetRandomItem()
    {
        //generate a random number for this range
        //50% for common
        //30% for rare
        //15% for epic
        //5% for legendary

        int random_number = Random.Range(1, 101);

        if(random_number < 50)
        {
            return common_items[Random.Range(0, common_items.Count)];
        }else if(random_number < 80)
        {
            return rare_items[Random.Range(0, rare_items.Count)];
        }else if(random_number < 95)
        {
            return epic_items[Random.Range(0, epic_items.Count)];
        }
        else
        {
            return legendary_items[Random.Range(0, legendary_items.Count)];
        }
    }

}
