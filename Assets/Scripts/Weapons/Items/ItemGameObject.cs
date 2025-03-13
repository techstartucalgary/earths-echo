using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class ItemGameObject : MonoBehaviour
{
    public ItemSO item;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer==null){
            Debug.LogError("No sprite renderer present");
        }
    }

    public void Start()
    {
        if(item!=null && spriteRenderer !=null && transform !=null){
            spriteRenderer.sprite = item.itemIcon;
        }
        else{
            Debug.LogWarning("No image in scriptable object");
        }
    }
}
