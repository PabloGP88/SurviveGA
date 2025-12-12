using System.Collections.Generic;
using UnityEngine;

public class Dna : MonoBehaviour
{
    public BunnyNeuralNetwork bunnyBrain;
    
    public float stepSize;
    
    public static readonly Vector2[] possibleDirections = new Vector2[]
    {
        Vector2.up,                     // Up
        Vector2.down,                   // Down
        Vector2.left,                   // Left
        Vector2.right,                  // Right
    };
    
    public Vector2[] visionDirections = new Vector2[]
    {
        Vector2.up,                         // Up
        new Vector2(1, 1).normalized,       // Up-Right
        new Vector2(-1, 1).normalized,      // Up-Left
        Vector2.right,                      // Right
        new Vector2(1, -1).normalized,      // Down-Right
        Vector2.left,                       // Left
        new Vector2(-1, -1).normalized,     // Down-Left
        Vector2.down,                       // Down
    };

    public float visionRange = 5.0f;

    public float health;
    public float hunger;
    
    public float fitness;

    private const float MAX_HEALTH = 100;
    private const float MAX_HUNGER = 100;
    
    public float GetMaxHunger() { return MAX_HUNGER; }
    public float GetMaxHealth() { return MAX_HEALTH; }

    public int inputSize;
    
    public List<Vector2> movesTaken = new List<Vector2>();
    
    public void InitData()
    {
        stepSize = 0.1f;
        fitness = 0;
        health = MAX_HEALTH;
        hunger = MAX_HUNGER;
    
        inputSize = visionDirections.Length + 2; 
        int hiddenSize = 6; 
        int outputSize = possibleDirections.Length;


        bunnyBrain = new BunnyNeuralNetwork();
        bunnyBrain.Init(inputSize, hiddenSize, outputSize);
    }

    public int ChooseDirection(float[] inputs)
    {
        float[] outputs = bunnyBrain.Forward(inputs);

        // Pick the index of the highest output
        int bestIndex = 0;
        float bestValue = outputs[0];
        for (int i = 1; i < outputs.Length; i++)
        {
            if (outputs[i] > bestValue)
            {
                bestValue = outputs[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }
    
    public void Mutate(float mutationRate)
    {
        // Mutate the neural network weights
        bunnyBrain.Mutate(mutationRate, mutationStrength: 0.5f);
    }
}