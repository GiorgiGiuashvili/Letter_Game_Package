using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public static List<Slot> AllSlots { get; private set; } = new List<Slot>(); 

    public bool IsOccupied;
    private TagValue _tagValue;

    private void Awake()
    {
        if (!AllSlots.Contains(this))
        {
            AllSlots.Add(this);
        }


        _tagValue = GetComponent<TagValue>();

    }

    private void OnDestroy()
    {
        AllSlots.Remove(this);
    }

}