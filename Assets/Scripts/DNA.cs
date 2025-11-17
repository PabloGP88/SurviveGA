using System.Collections.Generic;
using UnityEngine;
public class Dna : MonoBehaviour
{
    // Movement genes
    public Vector2[] directions;
    public float stepSize;

   
    public float energy;
    public float fitness;
    
    public static readonly Vector2[] possibleDirections = new Vector2[]
    {
        Vector2.up,              // Up
        Vector2.down,            // Down
        Vector2.left,            // Left
        Vector2.right,           // Right
        new Vector2(1, 1).normalized,   // Up-Right
        new Vector2(-1, 1).normalized,  // Up-Left
        new Vector2(1, -1).normalized,  // Down-Right
        new Vector2(-1, -1).normalized  // Down-Left
    };
    
    public List<Vector2> movesTaken = new List<Vector2>();
    public void InitData()
    {
        directions = new Vector2[3000];
        stepSize = 25f;
        fitness = 0;
        
        for (int i = 0; i < directions.Length; i++)
        {
            directions[i] = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }
    
    public void Mutate(float mutationRate = 0.05f)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                // Slightly modify the direction 
                Vector2 mutation = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                directions[i] = (directions[i] + mutation).normalized;
            }
        }
    }

}