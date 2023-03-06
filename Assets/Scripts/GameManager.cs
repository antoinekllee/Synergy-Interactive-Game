using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DragDrop[] agentItems = null; 

    List<DragDrop> group1Items = new List<DragDrop>();
    List<DragDrop> group2Items = new List<DragDrop>();

    [Space (8)]
    [SerializeField] private ItemSlot itemSlot1 = null; // Group 1
    [SerializeField] private ItemSlot itemSlot2 = null; // Group 2

    [Space (8)]
    [SerializeField] private int groupSize = 4; 

    [Space (8)]
    [SerializeField, NonReorderable] Color[] itemColours = new Color[10]; 

    private void Start ()
    {
        agentItems = FindObjectsOfType<DragDrop>(); 

        for (int i = 0; i < agentItems.Length; i++)
            agentItems[i].UpdateStats ("Agent " + (i + 1).ToString());
    }

    public void Evaluate ()
    {
        group1Items.Clear();
        group2Items.Clear();

        for (int i = 0; i < agentItems.Length; i++)
        {
            // Check if the currentAgentItem is a child of the item slot, even if not a direct child
            if (agentItems[i].transform.IsChildOf(itemSlot1.transform))
                group1Items.Add(agentItems[i]);
            else if (agentItems[i].transform.IsChildOf(itemSlot2.transform))
                group2Items.Add(agentItems[i]);
        }

        if (group1Items.Count != groupSize || group2Items.Count != groupSize)
        {
            Debug.Log("There must be " + groupSize + " agents in each group.");
            return;
        }

        // Print the name of each group 1 item
        Debug.Log ("Group 1 Items: "); 
        for (int i = 0; i < group1Items.Count; i++)
            Debug.Log(group1Items[i].agentName);

        // Print the name of each group 2 item
        Debug.Log ("Group 2 Items: "); 
        for (int i = 0; i < group2Items.Count; i++)
            Debug.Log(group2Items[i].agentName);
    }
}