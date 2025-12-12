using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private int populationSize;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float generationTime = 60f;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Slider _speedSlider;
    
    [Header("Genetic Algorithm Settings")]
    [SerializeField] private float mutationRate = 0.05f;
    [SerializeField] private int eliteCount = 2;
    
    private List<GameObject> _population = new List<GameObject>();
    private int _currentGeneration = 1;
    private float _timer = 0f;
    private float _bestFitnessLastGen = 0f;
    private float _averageFitnessLastGen = 0f;
    
    // Data tracking for export
    private List<float> _bestFitnessHistory = new List<float>();
    private List<float> _averageFitnessHistory = new List<float>();

    private void Start()
    {
        _speedSlider.value = Time.timeScale;
        CreatePopulation();
        UpdateDebugText();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= generationTime)
        {
            NextGeneration();
        }
        
        UpdateDebugText();
    }

    private void CreatePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject agent = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            agent.GetComponent<Dna>().InitData();
            _population.Add(agent);
        }
    }

    private void NextGeneration()
    {
        // Sort by fitness
        _population = _population.OrderByDescending(a => a.GetComponent<Dna>().fitness).ToList();
        _bestFitnessLastGen = _population[0].GetComponent<Dna>().fitness;
        _averageFitnessLastGen = _population.Average(a => a.GetComponent<Dna>().fitness);
        
        // Record data for export
        _bestFitnessHistory.Add(_bestFitnessLastGen);
        _averageFitnessHistory.Add(_averageFitnessLastGen);
        
        // Export at generation 100
        if (_currentGeneration == 100)
        {
            ExportData();
        }
        
        List<GameObject> newPopulation = new List<GameObject>();
        
        // ELITISM: Keep the best N agents (exact copies)
        for (int i = 0; i < eliteCount && i < _population.Count; i++)
        {
            GameObject elite = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            Dna eliteDna = elite.GetComponent<Dna>();
            eliteDna.InitData();
            
            Dna bestDna = _population[i].GetComponent<Dna>();
            
            // Copy the neural network weights from the best agent
            eliteDna.bunnyBrain.CopyWeights(bestDna.bunnyBrain);
            
            eliteDna.fitness = 0;

            // Mark elite agents with green color
            elite.GetComponent<SpriteRenderer>().color = Color.green;
            
            newPopulation.Add(elite);
        }
        
        // Create children for the rest of the population
        while (newPopulation.Count < populationSize)
        {
            // Tournament selection for parent diversity
            GameObject parent1 = TournamentSelection();
            GameObject parent2 = TournamentSelection();
            
            // Create child through crossover
            GameObject child = Crossover(parent1, parent2);
            
            // Mutate the child
            child.GetComponent<Dna>().Mutate(mutationRate);
            
            newPopulation.Add(child);
        }
        
        // Clean up old population
        foreach (GameObject agent in _population)
        {
            Destroy(agent);
        }
        
        _population = newPopulation;
        _currentGeneration++;
        _timer = 0f;
    }
    

    private GameObject TournamentSelection(int tournamentSize = 5)
    {
        GameObject best = _population[Random.Range(0, _population.Count)];
        float bestFitness = best.GetComponent<Dna>().fitness;
        
        for (int i = 1; i < tournamentSize; i++)
        {
            GameObject candidate = _population[Random.Range(0, _population.Count)];
            float candidateFitness = candidate.GetComponent<Dna>().fitness;
            
            if (candidateFitness > bestFitness)
            {
                best = candidate;
                bestFitness = candidateFitness;
            }
        }
        
        return best;
    }
    

    private GameObject Crossover(GameObject parent1, GameObject parent2)
    {
        GameObject child = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
        Dna childDna = child.GetComponent<Dna>();
        childDna.InitData();
        
        Dna parent1Dna = parent1.GetComponent<Dna>();
        Dna parent2Dna = parent2.GetComponent<Dna>();
        
        // Crossover neural network weights
        childDna.bunnyBrain.CrossoverFrom(parent1Dna.bunnyBrain, parent2Dna.bunnyBrain);
        
        return child;
    }

    private void UpdateDebugText()
    {
        float currentBestFitness = _population.Count > 0 
            ? _population.Max(a => a.GetComponent<Dna>().fitness) 
            : 0f;
        
        debugText.text = $"Generation: {_currentGeneration}\n" +
                         $"Time: {generationTime - _timer:F1}s\n" +
                         $"Best Fitness (Current): {currentBestFitness:F1}\n" +
                         $"Best Fitness (Last Gen): {_bestFitnessLastGen:F1}\n" +
                         $"Avg Fitness (Last Gen): {_averageFitnessLastGen:F1}\n" +
                         $"Population Size: {populationSize}\n" +
                         $"Speed x{_speedSlider.value:F1}"; 

        Time.timeScale = _speedSlider.value;
    }
    
    private void ExportData()
    {
        StringBuilder csv = new StringBuilder();
        csv.AppendLine("Generation,Best Fitness,Average Fitness");
        
        for (int i = 0; i < _bestFitnessHistory.Count; i++)
        {
            csv.AppendLine($"{i + 1},{_bestFitnessHistory[i]:F2},{_averageFitnessHistory[i]:F2}");
        }
        
        string path = Path.Combine(Application.dataPath, "generation_data.csv");
        File.WriteAllText(path, csv.ToString());
        
        Debug.Log($"Data exported to: {path}");
    }
}