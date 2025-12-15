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
    public int poissonEaten = 0;

    public int stepsToFirstFood = 0;
    public bool firstFood = false;
    public bool hasDied = false;
    
    public bool isElite = false;
    
    public HashSet<Vector2> explorationDone = new HashSet<Vector2>();
    
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
    
    public void Mutate(float mutationRate)
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
    
    public void CalculateFitness(int moveIndex)
    {
        fitness = 0f;
        
        
        float timeAlive = (float)moveIndex / directions.Length;
        // I need to clamp it so the fitness makes sense when analyzing 
        timeAlive = Mathf.Clamp01(timeAlive);

        // Death is = REALLY BAD
        if (hasDied)
            timeAlive *= 0.05f;
        
        float stepsToFood = 0f;
        
        if (firstFood)
        {
            stepsToFood = (1f / (stepsToFirstFood + 1f)) * 10;
            stepsToFood = Mathf.Clamp01(stepsToFood);
        }
        
        float foodValue = Mathf.Log(foodEaten + 1, 2) / 4f; 
        foodValue = Mathf.Clamp01(foodValue);
        
        float poissonValue = 1f - Mathf.Exp(-poissonEaten * 0.5f);
        poissonValue = Mathf.Clamp01(poissonValue);

        float survivedEating = 0;
        

        float explored = Mathf.Clamp01(explorationDone.Count / 360f);
        
        // Im using a max fitness value of 100, so now I multiplied the clamp values to the 
        // importance percentage I want them to have in the final fitness
        
        fitness =  (timeAlive * 25.0f) + (foodValue * 25.0f) + (explored * 25) + (stepsToFood * 25.0f);
    }

}