using System.Collections.Generic;
using UnityEngine;
public class Dna : MonoBehaviour
{
    // Movement genes
    public Vector2[] directions;
    public float stepSize;

    private const int Energy = 3000;
    public float fitness;
    
    public int foodEaten = 0;

    public int stepsToFirstFood = 0;
    public bool firstFood = false;
    public bool isElite = false;
    
    public List<Vector2> movesTaken = new List<Vector2>();
    public void InitData()
    {
        directions = new Vector2[Energy];
        stepSize = 0.5f;
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
                Vector2 mutation = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                directions[i] = (directions[i] + mutation).normalized;
            }
        }
    }
    
    public void CalculateFitness(int _moveIndex)
    {
        fitness = 0f;
        
        // reward food eating and surviving longer
        fitness += foodEaten * 10f;
        fitness += (_moveIndex / 100f) * 10f;
        
        // Reward getting food early to avoid none sense movement 
        if (firstFood)
        {
            float efficiencyBonus = Mathf.Max(0, 10f - (stepsToFirstFood / 60f));
            fitness += efficiencyBonus;
        }
        
        if (_moveIndex >= directions.Length)
        {
            fitness += 30f; 
        }
        
        if (foodEaten == 0)
        {
            fitness -= 30f;
        }
    }

}