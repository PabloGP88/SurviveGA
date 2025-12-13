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
                // Slightly modify the direction 
                Vector2 mutation = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                directions[i] = (directions[i] + mutation).normalized;
            }
        }
    }
    
    public void CalculateFitness()
    {
        fitness = 0f;
        
        // Reward for total food eaten (primary objective)
        fitness += foodEaten * 10f;
        
        // Bonus for finding food quickly (efficiency reward)
        if (firstFood)
        {
            // The earlier they find food, the higher the bonus (max 5 points if found immediately)
            float efficiencyBonus = Mathf.Max(0, 5f - (stepsToFirstFood / 600f));
            fitness += efficiencyBonus;
        }
        
        // Extra bonus for eating multiple foods (rewards sustained foraging)
        if (foodEaten > 1)
        {
            fitness += (foodEaten - 1) * 2f; // Exponential-ish growth for multiple foods
        }
        
        // Small penalty if no food was found at all (encourages exploration)
        if (foodEaten == 0)
        {
            fitness -= 2f;
        }
    }

}