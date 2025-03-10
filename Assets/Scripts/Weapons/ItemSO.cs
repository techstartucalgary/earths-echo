using UnityEngine;

public abstract class ItemSO : ScriptableObject
{
    [Header("Item Data")]
    [Tooltip("The name of the item.")]
    public string itemName;
    
    [TextArea]
    [Tooltip("A detailed description of the item.")]
    public string description;
    
    [Tooltip("The icon representing the item in the UI.")]
    public Sprite itemIcon;
    
    [Header("Stacking Settings")]
    [Tooltip("Can this item be stacked?")]
    public bool stackable = true;

    // bool for if the player can use the item using 'T'. Ex: grappling hook
    public bool usable = true;
    
    [Tooltip("Maximum number of items allowed in a stack.")]
    public int maxStack = 99;
    
    [Header("Item Audio")]
    [Tooltip("Audio clips associated with item actions (e.g., swing, hit).")]
    public AudioClip[] audioClips;

    public float itemScale = 1f;

}
