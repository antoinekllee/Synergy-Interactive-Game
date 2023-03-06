using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Transform itemParent = null;
    [SerializeField] private bool isGroup1 = false; 

    private GameManager gameManager = null; 

    private void Start ()
    {
        gameManager = FindObjectOfType<GameManager>(); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.transform.SetParent(itemParent);
        }
    }
}