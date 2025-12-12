using UnityEngine;

// This is NOT a MonoBehaviour - it's just a regular class
public class BunnyNeuralNetwork
{
    private int inputSize;
    private int hiddenSize;
    private int outputSize;

    private float[,] W1; // input → hidden
    private float[,] W2; // hidden → output
    private float[] b1;
    private float[] b2;

    public void Init(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        W1 = RandomMatrix(inputSize, hiddenSize);
        W2 = RandomMatrix(hiddenSize, outputSize);
        b1 = new float[hiddenSize];
        b2 = new float[outputSize];
    }

    public float[] Forward(float[] inputs)
    {
        // Hidden layer
        float[] hidden = new float[hiddenSize];
        for (int j = 0; j < hiddenSize; j++)
        {
            float sum = b1[j];
            for (int i = 0; i < inputSize; i++)
                sum += inputs[i] * W1[i, j];
            hidden[j] = Mathf.Max(0, sum); // ReLU
        }

        // Output layer
        float[] outputs = new float[outputSize];
        for (int k = 0; k < outputSize; k++)
        {
            float sum = b2[k];
            for (int j = 0; j < hiddenSize; j++)
                sum += hidden[j] * W2[j, k];
            outputs[k] = 1f / (1f + Mathf.Exp(-sum)); // Sigmoid
        }
        return outputs;
    }

    private float[,] RandomMatrix(int rows, int cols)
    {
        float[,] m = new float[rows, cols];
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            m[i, j] = Random.Range(-1f, 1f);
        return m;
    }
    

    public void CopyWeights(BunnyNeuralNetwork other)
    {
        // Copy W1 (input → hidden weights)
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                W1[i, j] = other.W1[i, j];
            }
        }
        
        // Copy W2 (hidden → output weights)
        for (int j = 0; j < hiddenSize; j++)
        {
            for (int k = 0; k < outputSize; k++)
            {
                W2[j, k] = other.W2[j, k];
            }
        }
        
        // Copy biases
        for (int j = 0; j < hiddenSize; j++)
        {
            b1[j] = other.b1[j];
        }
        
        for (int k = 0; k < outputSize; k++)
        {
            b2[k] = other.b2[k];
        }
    }
    
    /// <summary>
    /// Mutates weights and biases with given probability
    /// </summary>
    public void Mutate(float mutationRate, float mutationStrength = 0.5f)
    {
        // Mutate W1
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                if (Random.value < mutationRate)
                {
                    W1[i, j] += Random.Range(-mutationStrength, mutationStrength);
                }
            }
        }
        
        // Mutate W2
        for (int j = 0; j < hiddenSize; j++)
        {
            for (int k = 0; k < outputSize; k++)
            {
                if (Random.value < mutationRate)
                {
                    W2[j, k] += Random.Range(-mutationStrength, mutationStrength);
                }
            }
        }
        
        // Mutate b1
        for (int j = 0; j < hiddenSize; j++)
        {
            if (Random.value < mutationRate)
            {
                b1[j] += Random.Range(-mutationStrength, mutationStrength);
            }
        }
        
        // Mutate b2
        for (int k = 0; k < outputSize; k++)
        {
            if (Random.value < mutationRate)
            {
                b2[k] += Random.Range(-mutationStrength, mutationStrength);
            }
        }
    }
    
    /// <summary>
    /// Creates child weights by randomly selecting from two parents (uniform crossover)
    /// </summary>
    public void CrossoverFrom(BunnyNeuralNetwork parent1, BunnyNeuralNetwork parent2)
    {
        // Crossover W1
        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                W1[i, j] = (Random.value < 0.5f) ? parent1.W1[i, j] : parent2.W1[i, j];
            }
        }
        
        // Crossover W2
        for (int j = 0; j < hiddenSize; j++)
        {
            for (int k = 0; k < outputSize; k++)
            {
                W2[j, k] = (Random.value < 0.5f) ? parent1.W2[j, k] : parent2.W2[j, k];
            }
        }
        
        // Crossover b1
        for (int j = 0; j < hiddenSize; j++)
        {
            b1[j] = (Random.value < 0.5f) ? parent1.b1[j] : parent2.b1[j];
        }
        
        // Crossover b2
        for (int k = 0; k < outputSize; k++)
        {
            b2[k] = (Random.value < 0.5f) ? parent1.b2[k] : parent2.b2[k];
        }
    }
}