using UnityEngine;

public class Agent : MonoBehaviour
{
    private Dna dna;
    
    private bool isActive = true;
    private float hungerDecay = 5f;
    private float healthDecreaseRate = 10f;
    private float timer = 0f;
    private float timeToMove = 0.1f; // SECONDS

    private Vector2 moveDirection;
    
    private void Start()    
    {
        dna = GetComponent<Dna>();
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;
        dna.fitness += Time.deltaTime * 1000.0f;
    
        HealthManagement();
    
        if (timer >= timeToMove)
        {
            // Pass context to DNA
            float hungerPercent = dna.hunger / dna.GetMaxHunger();
            Vector2 boundaryCenter = Vector2.zero; // Or your actual boundary/danger position
        
            int direction = dna.ChooseDirection(hungerPercent, transform.position, boundaryCenter);
        
            moveDirection = Dna.possibleDirections[direction] * dna.stepSize;
            transform.Translate(moveDirection);
        
            dna.movesTaken.Add(moveDirection);
            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("food"))
        {
            GrabbedFood();
        } else if (other.CompareTag("negative"))
        {
            InstaDead();
        } else if (other.CompareTag("poisson"))
        {
            GrabbedPoison();
        }
    }

    private void HealthManagement()
    {
        dna.hunger -= hungerDecay * Time.deltaTime;
        if (dna.hunger <= 0f)
        {
            dna.hunger = 0f;
        }

        if (dna.hunger < dna.GetMaxHunger()/2)
        {
            print("Starving");
            dna.health -= healthDecreaseRate * Time.deltaTime;
        }

        if (dna.health <= 0)
        {
            isActive = false;
            dna.fitness -= 100f;
        }
    }

    private void GrabbedFood()
    {
        dna.hunger += 100f;
        dna.health += 5f;
        dna.fitness += 1f;

        if (dna.hunger > dna.GetMaxHunger())
        {
            dna.hunger = dna.GetMaxHunger();
        }

        if (dna.health > dna.GetMaxHealth())
        {
            dna.health = dna.GetMaxHealth();
        }
    }

    private void GrabbedPoison()
    {
        dna.hunger -= 10f;
        dna.health -= 5f;
        dna.fitness -= 25;

        if (dna.hunger < 0)
        {
            dna.hunger = 0;
        }

        if (dna.health < 0)
        {
            dna.health = 0;
        }
    }

    private void InstaDead()
    {
        dna.fitness -= 500f; // Penalty, not death
        dna.health = 0f;   // Damage, not instant death
        
    }
    public void ResetAgent()
    {
        timer = 0f;  
        dna.fitness = 0;
        dna.health = dna.GetMaxHealth();
        dna.hunger = dna.GetMaxHunger();
        isActive = true;
    }
}
