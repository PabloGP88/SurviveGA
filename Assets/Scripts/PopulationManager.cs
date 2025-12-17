using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private Predator[] predators;
    [SerializeField] private int populationSize;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private float generationTime = 60f;
    
    [Header("Genetic Algorithm Settings")]
    [SerializeField] private float mutationRate = 0.05f;
    [SerializeField] private int eliteCount = 2;
    
    private List<GameObject> _population = new List<GameObject>();
    private int _currentGeneration = 1;
    private float _timer = 0f;
    private float _bestFitnessLastGen = 0f;
    private float _averageFitnessLastGen = 0f;
    private int _bestFitnessSteps = 0;
    private int _averageFitnessSteps = 0;
    private int _bestFitnessFoodCount = 0;
    private int _averageFitnessFoodCount = 0;
    private int _bestFitnessDead = 0;
    private int _averageFitnessDead = 0;

    private List<GenerationData> _generationHistory = new List<GenerationData>();
    
    [System.Serializable]
    public class GenerationData
    {
        public int generation;
        public float bestFitness;
        public float averageFitness;
        public int bestStepsToFirstFood;
        public int averageStepsToFirstFood;
        public int bestFoodCount;
        public int averageFoodCount;

        public GenerationData(
            int gen,
            float best,
            float avg,
            int stepsBest,
            int stepsAverage,
            int bestFood,
            int averageFood
            )
        {
            generation = gen;
            bestFitness = best;
            averageFitness = avg;
            bestStepsToFirstFood = stepsBest;
            averageStepsToFirstFood = stepsAverage;
            bestFoodCount = bestFood;
            averageFoodCount = averageFood;
        }
    }

    private void Start()
    {
        CreatePopulation();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        if (_timer >= generationTime)
        {
            NextGeneration();
        }
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
        foreach (GameObject agent in _population)
        {
            agent.GetComponent<Dna>().CalculateFitness(agent.GetComponent<Agent>().GetTotalMoves());
        }
        
        _population = _population.OrderByDescending(a => a.GetComponent<Dna>().fitness).ToList();
        
        _bestFitnessLastGen = _population[0].GetComponent<Dna>().fitness;
        _averageFitnessLastGen = _population.Average(a => a.GetComponent<Dna>().fitness);
        
        _bestFitnessSteps = _population[0].GetComponent<Dna>().stepsToFirstFood;
        _averageFitnessSteps = (int)_population.Average(a => a.GetComponent<Dna>().stepsToFirstFood);

        _bestFitnessFoodCount = _population[0].GetComponent<Dna>().foodEaten;
        _averageFitnessFoodCount = (int)_population.Average(a => a.GetComponent<Dna>().foodEaten);
        
        _bestFitnessDead = _population[0].GetComponent<Dna>().hasDied ? 1 : 0;
        _averageFitnessDead = (int)_population.Average(a => a.GetComponent<Dna>().hasDied ? 1 : 0);
        
        _generationHistory.Add(new GenerationData(
            _currentGeneration,
            _bestFitnessLastGen,
            _averageFitnessLastGen,
            _bestFitnessSteps,
            _averageFitnessSteps,
            _bestFitnessFoodCount,
            _averageFitnessFoodCount
            )
        );
        
        List<GameObject> newPopulation = new List<GameObject>();
        
        for (int i = 0; i < eliteCount && i < _population.Count; i++)
        {
            GameObject elite = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            Dna eliteDna = elite.GetComponent<Dna>();
            eliteDna.InitData();
            
            Dna bestDna = _population[i].GetComponent<Dna>();
            eliteDna.stepSize = bestDna.stepSize;
            System.Array.Copy(bestDna.directions, eliteDna.directions, eliteDna.directions.Length);
            eliteDna.fitness = 0;
            
            elite.GetComponent<SpriteRenderer>().color = Color.cyan;
            eliteDna.isElite = true;
            
            newPopulation.Add(elite);
        }
        
        while (newPopulation.Count < populationSize)
        {
            GameObject parent1 = TournamentSelection();
            GameObject parent2 = TournamentSelection();
            
            GameObject child = Crossover(parent1, parent2);
            child.GetComponent<Dna>().Mutate(mutationRate);
            
            newPopulation.Add(child);
        }

        Clean();
        
        _population = newPopulation;
        _currentGeneration++;
        _timer = 0f;
    }
    
    private GameObject TournamentSelection(int tournamentSize = 4)
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
    
    private GameObject RouletteWheelSelection()
    {
        float totalFitness = 0f;
        foreach (GameObject agent in _population)
        {
            totalFitness += agent.GetComponent<Dna>().fitness;
        }
        
        float randomValue = Random.Range(0f, totalFitness);
        float wheelSpin = 0f;

        foreach (GameObject agent in _population)
        {
            wheelSpin += agent.GetComponent<Dna>().fitness;
            if (wheelSpin >= randomValue)
            {
                return agent;
            }
        }
        
        return _population[_population.Count - 1];
    }
    
    private GameObject Crossover(GameObject parent1, GameObject parent2)
    {
        GameObject child = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
        Dna childDna = child.GetComponent<Dna>();
        childDna.InitData();
        
        Dna parent1Dna = parent1.GetComponent<Dna>();
        Dna parent2Dna = parent2.GetComponent<Dna>();
        
        int length = childDna.directions.Length;  
        int point1 = Random.Range(0, length);
        int point2 = Random.Range(0, length);
        
        if (point1 > point2)
        {
            (point1, point2) = (point2, point1);
        }
        
        for (int j = 0; j < length; j++)
        {
            
            //  // Two point
            // childDna.directions[j] = (j >= point1 && j <= point2)
            //     ? parent1Dna.directions[j]
            //     : parent2Dna.directions[j];
            
            //Uniform
             childDna.directions[j] = (j%2  == 0)
                 ? parent1Dna.directions[j]
                 : parent2Dna.directions[j];
            
        }
        
        return child;
    }

    private void Clean()
    {
        foreach (GameObject agent in _population)
        {
            Destroy(agent);
        }

        if (predators.Length > 0)
        {
            foreach (Predator predator in predators)
            {
                predator.Reset();
            }
        }
    }
    
    public int GetCurrentGeneration() => _currentGeneration; 
    public float GetTimeRemaining() => generationTime - _timer; 
    public float GetMutationRate() => mutationRate; 
    public float GetBestFitnessLastGen() => _bestFitnessLastGen; 
    public float GetAverageFitnessLastGen() => _averageFitnessLastGen; 
    public int GetPopulationSize() => populationSize;

    public void ToggleEliteOnly(bool showOnlyElites)
    {
        foreach (GameObject agent in _population)
        {
            Dna dna = agent.GetComponent<Dna>();
            SpriteRenderer sprite = agent.GetComponent<SpriteRenderer>();
            if (showOnlyElites)
            {
                sprite.enabled = dna.isElite;
            }
            else
            {
                sprite.enabled = true;
            }
        }
    }
    public void ExportToCSV()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"GeneticAlgorithm_Export_{timestamp}.csv";
        string filepath = System.IO.Path.Combine(Application.dataPath, filename);
        
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filepath))
        {
            writer.WriteLine(
                "Generation," +
                "BestFitness,AverageFitness," +
                "BestStepsToFirstFood,AverageStepsToFirstFood," +
                "BestFoodCount,AverageFoodCount," +
                "BestAgentDied"
            );
            
            foreach (GenerationData data in _generationHistory)
            {
                writer.WriteLine(
                    $"{data.generation}," +
                    $"{data.bestFitness}," +
                    $"{data.averageFitness}," +
                    $"{data.bestStepsToFirstFood}," +
                    $"{data.averageStepsToFirstFood}," +
                    $"{data.bestFoodCount}," +
                    $"{data.averageFoodCount}," 
                );
            }
        }
        
        Debug.Log($"CSV exported successfully to: {filepath}");
    }
}
