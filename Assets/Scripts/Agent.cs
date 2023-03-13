using UnityEngine; 
using System;
using Random = UnityEngine.Random;

public class AgentClass : MonoBehaviour
{
    [Serializable]
    public class Agent : IEquatable <Agent>
    {
        public string name = "Name"; 
        public int index = 0; 
        [Space (8)]
        [Range (-1f, 1f)] public float sn = 0f; 
        [Range (-1f, 1f)] public float tf = 0f; 
        [Range (-1f, 1f)] public float ei = 0f; 
        [Range (-1f, 1f)] public float pj = 0f; 

        public int gender = 0; 

        public Agent (string name)
        {
            this.name = name; 
            
            gender = Random.Range(0, 2);

            float[] values = new float []{ -1f, -0.8f, -0.6f, -0.4f, -0.2f, 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f }; 
            int index = Random.Range(0, values.Length);
            sn = values[index];
            
            index = Random.Range(0, values.Length);
            tf = values[index];
            
            index = Random.Range(0, values.Length);
            ei = values[index];
            
            index = Random.Range(0, values.Length);
            pj = values[index];
        }

        public bool Equals (Agent other) 
        {
            return index == other.index; 
        }

        public override bool Equals(object obj) => Equals(obj as Agent); 
        public override int GetHashCode() => (sn, tf, ei, pj, gender).GetHashCode();
    }
}