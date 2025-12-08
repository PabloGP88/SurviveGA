using System.Collections.Generic;
using UnityEngine;
public class Dna : MonoBehaviour
{
    public float stepSize;
    
    public static readonly Vector2[] possibleDirections = new Vector2[]
    {
        Vector2.zero,                   // do nothing
        Vector2.up,                     // Up
        Vector2.down,                   // Down
        Vector2.left,                   // Left
        Vector2.right,                  // Right
        new Vector2(1, 1).normalized,   // Up-Right
        new Vector2(-1, 1).normalized,  // Up-Left
        new Vector2(1, -1).normalized,  // Down-Right
        new Vector2(-1, -1).normalized  // Down-Left
    };
    
    public float[] weights =  new float[possibleDirections.Length];

    public float health;
    public float hunger;
    
    public float fitness;

    
    private const float  MAX_HEALTH =  100;
    private const float  MAX_HUNGER =  100;
    
    public float GetMaxHunger() { return MAX_HUNGER;}
    public float GetMaxHealth() { return MAX_HEALTH;}
    
    public List<Vector2> movesTaken = new List<Vector2>();
    public void InitData()
    {
        stepSize = 0.1f;
        fitness = 0;
        health = MAX_HEALTH;
        hunger = MAX_HUNGER;
    
        // Ensure weights array is the correct size
        if (weights == null || weights.Length != possibleDirections.Length)
        {
            weights = new float[possibleDirections.Length];
        }
    
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.Range(0.5f, 1.0f);
        }
    
        NormalizeWeights();
    }
    

    public int ChooseDirection(float hungerPercent, Vector2 position, Vector2 boundaryCenter)
    {
        float[] adjustedWeights = (float[])weights.Clone();
    
        // When full, strongly prefer staying or moving away from edges
        if (hungerPercent > 0.7f)
        {
            adjustedWeights[0] *= 3f; // Boost "stay still"
        
            // Reduce weights that move toward boundaries
            Vector2 toBoundary = (boundaryCenter - position).normalized;
            for (int i = 1; i < possibleDirections.Length; i++)
            {
                float alignment = Vector2.Dot(possibleDirections[i], toBoundary);
                if (alignment > 0.5f) // Moving toward boundary
                    adjustedWeights[i] *= 0.2f;
            }
        }
    
        return SelectFromWeights(adjustedWeights);
    }

// Extract your existing wheel selection logic
    private int SelectFromWeights(float[] weightsToUse)
    {
        float wheelTotalWeight = 0;

        foreach (float weight in weightsToUse)
        {
            wheelTotalWeight += weight;
        }
    
        float pieceOfWheel = Random.Range(0, wheelTotalWeight);
        float sum = 0;
    
        for (int i = 0; i < weightsToUse.Length; i++)
        {
            sum += weightsToUse[i];
            if (pieceOfWheel <= sum)
            {
                return i;
            }
        }
    
        return 0; // Fallback
    }
    public void Mutate(float mutationRate = 0.05f)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                // Modify the value of the weight
                float mutation = Random.Range(-0.5f, 0.5f);
                weights[i] += mutation;

                if (weights[i] <= 0)
                {
                    weights[i] = 0.01f;
                }
            }
        }
        
        NormalizeWeights();
    }
    
    private void NormalizeWeights()
    {
        // This function normalize weights so now the max is 1.0 and weights actually mean probabilities
        // For example after normalizing 0.25 = 25%
        // comment for future pablo
        
        float total = 0;
        
        foreach (float weight in weights)
        {
            total += weight;
        }
        
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] /= total;  
        }
        
    }

}