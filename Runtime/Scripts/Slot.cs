using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool IsOccupied;
    private TagValue _tagValue;

    private void Awake()
    {
        _tagValue = GetComponent<TagValue>();
    }

}