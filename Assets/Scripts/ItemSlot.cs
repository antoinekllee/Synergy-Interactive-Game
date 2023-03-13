using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private Transform itemParent = null;
    private GameManager gameManager = null; 

    private void Start ()
    {
        gameManager = FindObjectOfType<GameManager>(); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || !eventData.pointerDrag.gameObject.TryGetComponent<DragDrop>(out DragDrop dragDrop))
            return; 

        eventData.pointerDrag.transform.SetParent(itemParent);
        gameManager.Evaluate();
    }
}