using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] public List<GameObject> items;

    public List<GameObject> getList()
    {
        return items;
    }

}
