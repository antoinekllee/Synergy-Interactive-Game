using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; 
using static AgentClass;
using UnityEngine.UI;
using System.Collections;
using Michsky.MUIP; 

public class GameManager : MonoBehaviour
{
    [SerializeField] private WindowManager windowManager = null;
    [SerializeField] private DragDrop[] agentItems = null; 

    List<Agent> group1 = new List<Agent>();
    List<Agent> group2 = new List<Agent>();

    [Space (8)]
    [SerializeField] private ItemSlot itemSlot1 = null; // Group 1
    [SerializeField] private ItemSlot itemSlot2 = null; // Group 2

    [Space (8)]
    [SerializeField] private int groupSize = 4; 
    [SerializeField] private float gamma = 0.25f; 

    [Space (8)]
    [SerializeField, NonReorderable] Color[] itemColours = new Color[10]; 

    [Space (8)]
    [SerializeField] private TextMeshProUGUI utilityText = null;
    [SerializeField] private TextMeshProUGUI bestUtilityText = null;
    [SerializeField] private TextMeshProUGUI percentageText = null;

    [SerializeField] private TextMeshProUGUI totalScoreText = null; 
    
    [SerializeField] private ButtonManager submitButton = null;

    // private List<Agent> bestGroup1 = null;
    // private List<Agent> bestGroup2 = null;
    float currPerc = 0f; 
    float bestUtility = 0f; 

    int totalScore = 0;

    private void Start ()
    {
        agentItems = FindObjectsOfType<DragDrop>(); 

        // Randomise the order of itemColours
        for (int i = 0; i < itemColours.Length; i++)
        {
            Color temp = itemColours[i];
            int randomIndex = Random.Range(i, itemColours.Length);
            itemColours[i] = itemColours[randomIndex];
            itemColours[randomIndex] = temp;
        }

        for (int i = 0; i < agentItems.Length; i++)
            agentItems[i].UpdateStats ("Agent " + (i + 1).ToString(), itemColours[i]);


        FindBestPartition(); 
        Evaluate(); 

        bestUtilityText.text = "Target Score: " + bestUtility.ToString();

        windowManager = FindObjectOfType<WindowManager>();
        windowManager.OpenWindow("Chat");
    }

    public void FindBestPartition()
    {
        Agent[] agents = new Agent[agentItems.Length];
        for (int i = 0; i < agentItems.Length; i++)
        {
            agents[i] = agentItems[i].agent;
        }

        int groupSize = agents.Length / 2;

        List<List<Agent>> partitions = new List<List<Agent>>();
        GeneratePartitions(new List<Agent>(), agents.ToList(), groupSize, partitions);

        float maxUtility = float.MinValue;
        List<Agent> bestGroup1 = null;
        List<Agent> bestGroup2 = null;
        Dictionary<List<Agent>, float> groupUtilities = new Dictionary<List<Agent>, float>();

        foreach (List<Agent> partition in partitions)
        {
            List<Agent> group1 = partition;
            List<Agent> group2 = agents.Except(group1).ToList();

            float utility1;
            float utility2;

            if (groupUtilities.ContainsKey(group1))
                utility1 = groupUtilities[group1];
            else
            {
                utility1 = Utility(group1.ToArray());
                groupUtilities.Add(group1, utility1);
            }

            if (groupUtilities.ContainsKey(group2))
                utility2 = groupUtilities[group2];
            else
            {
                utility2 = Utility(group2.ToArray());
                groupUtilities.Add(group2, utility2);
            }

            float utility = utility1 * utility2;

            if (utility > maxUtility)
            {
                maxUtility = utility;
                bestGroup1 = group1;
                bestGroup2 = group2;
            }
        }

        bestUtility = Mathf.Round(maxUtility * 10000f) / 100f;
    }

    private void GeneratePartitions(List<Agent> currentGroup, List<Agent> remainingAgents, int groupSize, List<List<Agent>> partitions)
    {
        if (currentGroup.Count == groupSize)
        {
            partitions.Add(currentGroup);
            return;
        }

        for (int i = 0; i < remainingAgents.Count; i++)
        {
            Agent agent = remainingAgents[i];
            List<Agent> newGroup = new List<Agent>(currentGroup);
            newGroup.Add(agent);
            List<Agent> newRemainingAgents = new List<Agent>(remainingAgents);
            newRemainingAgents.RemoveAt(i);
            GeneratePartitions(newGroup, newRemainingAgents, groupSize, partitions);
        }
    }

    public bool Evaluate () // evaluate the utility of the current partition
    {
        List<DragDrop> group1Items = new List<DragDrop>();
        List<DragDrop> group2Items = new List<DragDrop>();

        currPerc = 0; 

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
            // Debug.Log ("GROUP 1 SIZE: " + group1Items.Count);
            // Debug.Log ("GROUP 2 SIZE: " + group2Items.Count);
            utilityText.text = "-"; 
            percentageText.text = "- %";

            return false;
        }

        group1 = new List<Agent>(); 
        group2 = new List<Agent>(); 

        // Populate group1 and group2
        // Debug.Log ("GROUP 1"); 
        for (int i = 0; i < group1Items.Count; i++)
        {
            group1.Add(group1Items[i].agent); 
            // Debug.Log(group1[i].name); 
        }

        // Debug.Log ("GROUP 2"); 
        for (int i = 0; i < group2Items.Count; i++)
        {
            group2.Add(group2Items[i].agent); 
            // Debug.Log(group2[i].name); 
        }

        float partitionUtility = Utility(group1.ToArray()) * Utility(group2.ToArray()); 
        float adjustedUtility = Mathf.Round(partitionUtility * 10000f) / 100f;
        utilityText.text = adjustedUtility.ToString();
        percentageText.text = (adjustedUtility / bestUtility * 100f).ToString("F2") + " %";

        currPerc = (adjustedUtility / bestUtility) * 100f;

        return true;
    }

    private float Utility (Agent [] team)
    {
        float diversity = GetDiversity(team); 

        float alpha = diversity / 3f; 
        float leadership = GetLeadership(team, alpha); 

        float beta = alpha * 3f; 
        float introversion = GetIntroversion(team, beta); 

        float genderBalance = GetGenderBalance(team); 

        return diversity + leadership + introversion + genderBalance; 
    }

    private float GetDiversity(Agent[] team)
    {
        return StandardDeviation(team.Select(x => x.sn).ToArray()) * StandardDeviation(team.Select(x => x.tf).ToArray());
    }

    private static float GetLeadership(Agent[] team, float alpha)
    {
        return Mathf.Max(Mathf.Max(team.Select(agent => Vector4.Dot(new Vector4(0f, -alpha, -alpha, alpha), new Vector4(agent.sn, agent.tf, agent.ei, agent.pj))).ToArray()), 0f);
    }

    private static float GetIntroversion(Agent[] team, float beta)
    {
        return Mathf.Max(Mathf.Max(team.Select(agent => Vector4.Dot(new Vector4(0f, 0f, beta, 0f), new Vector4(agent.sn, agent.tf, agent.ei, agent.pj))).ToArray()), 0f);
    }

    private float GetGenderBalance(Agent[] team)
    {
        return gamma * Mathf.Sin(Mathf.PI * GetGenderCount(team, 0) / (GetGenderCount(team, 0) + GetGenderCount(team, 1)));
    }

    private float StandardDeviation (float [] values) 
    {
        float average = values.Average(); 
        float sum = values.Sum (i => Mathf.Pow(i - average, 2f)); 
        return Mathf.Sqrt(sum / values.Length); 
    }

    private int GetGenderCount (Agent [] team, int gender) 
    {
        return team.Count(agent => agent.gender == gender); 
    }

    public void Reset()
    {
        for (int i = 0; i < agentItems.Length; i++)
            agentItems[i].ResetPosition(); 

        utilityText.text = "-";
        percentageText.text = "-%";

        FindBestPartition(); 
        Evaluate(); 

        bestUtilityText.text = "Target Score: " + bestUtility.ToString();
    }

    public void Submit()
    {
        if (!Evaluate()) return; 

        float additionalScore = currPerc;
        additionalScore /= 12f; 
        additionalScore = Mathf.Pow(additionalScore, 6f);
        additionalScore /= 10f; 
        additionalScore = Mathf.Round(additionalScore * 100f) / 100f;

        totalScore += (int)additionalScore;
        Debug.Log ("TOTAL SCORE: " + totalScore); 
        totalScoreText.text = "Score: " + totalScore.ToString();

        // Randomise the order of itemColours
        for (int i = 0; i < itemColours.Length; i++)
        {
            Color temp = itemColours[i];
            int randomIndex = Random.Range(i, itemColours.Length);
            itemColours[i] = itemColours[randomIndex];
            itemColours[randomIndex] = temp;
        }

        for (int i = 0; i < agentItems.Length; i++)
            agentItems[i].UpdateStats ("Agent " + (i + 1).ToString(), itemColours[i]);
            
        Reset(); 

        // Disable the submit button for 10 second
        submitButton.isInteractable = false;
        StartCoroutine(EnableSubmitButton());
    }

    private IEnumerator EnableSubmitButton()
    {
        yield return new WaitForSeconds(30f);
        submitButton.isInteractable = true;
    }
}