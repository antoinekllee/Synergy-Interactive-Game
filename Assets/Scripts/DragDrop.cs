using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{  
    [SerializeField] private Transform defaultParent = null; 
    [SerializeField] private Canvas canvas = null; 
    [SerializeField] private RectTransform dragBounds = null;

    [Space (8)]
    public string agentName = "Agent 1"; 
    public int gender = 0; 
    public int sn = 0; 
    public int tf = 0; 
    public int ei = 0; 
    public int pj = 0; 

    [Space (8)]
    [SerializeField] private TextMeshProUGUI nameText = null; 
    [SerializeField] private TextMeshProUGUI snText = null;
    [SerializeField] private TextMeshProUGUI tfText = null;
    [SerializeField] private TextMeshProUGUI eiText = null;
    [SerializeField] private TextMeshProUGUI pjText = null;

    [Space (8)]
    [SerializeField] private TextMeshProUGUI snSubtext = null; 
    [SerializeField] private TextMeshProUGUI tfSubtext = null;
    [SerializeField] private TextMeshProUGUI eiSubtext = null;
    [SerializeField] private TextMeshProUGUI pjSubtext = null;

    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void UpdateStats (string name)
    {
        agentName = name; 

        string genderStr = gender == 0 ? "Male" : "Female"; 
        nameText.text = agentName + " (" + genderStr + ")"; 
        
        sn = Random.Range(-10, 10); 
        tf = Random.Range(-10, 10); 
        ei = Random.Range(-10, 10); 
        pj = Random.Range(-10, 10); 

        snText.text = sn.ToString();
        tfText.text = tf.ToString();
        eiText.text = ei.ToString();
        pjText.text = pj.ToString();

        if (sn <= -3)
            snSubtext.text = "Sensing"; 
        else if (sn >= 3)
            snSubtext.text = "Intuition";
        else
            snSubtext.text = "Neutral";

        if (tf <= -3)
            tfSubtext.text = "Thinking";
        else if (tf >= 3)
            tfSubtext.text = "Feeling";
        else
            tfSubtext.text = "Neutral";

        if (ei <= -3)
            eiSubtext.text = "Extrovert";
        else if (ei >= 3)
            eiSubtext.text = "Introvert";
        else
            eiSubtext.text = "Neutral";

        if (pj <= -3)
            pjSubtext.text = "Perception";
        else if (pj >= 3)
            pjSubtext.text = "Judgement";
        else
            pjSubtext.text = "Neutral";
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f; 
        canvasGroup.blocksRaycasts = false; 
        transform.SetParent(defaultParent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Ensure that the item's rect transform cannot be dragged outside of the drag bounds even if this object has a different parent than the drag bounds.
        if (dragBounds != null)
        {
            Vector3[] corners = new Vector3[4];
            dragBounds.GetWorldCorners(corners);

            rectTransform.position = new Vector3(
                Mathf.Clamp(rectTransform.position.x, corners[0].x, corners[2].x),
                Mathf.Clamp(rectTransform.position.y, corners[0].y, corners[2].y),
                rectTransform.position.z
            );
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
