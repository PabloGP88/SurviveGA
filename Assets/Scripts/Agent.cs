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
    
    // Vision data 
    private float[] visionData;
    private void Start()    
    {
        dna = GetComponent<Dna>();
        visionData = new float[dna.visionDirections.Length];
    }

    private void Update()
    {
        if (!isActive)
            return;

        timer += Time.deltaTime;
        dna.fitness += 0.1f * Time.deltaTime;        
        HealthManagement();
    
        if (timer >= timeToMove)
        {
            // Perform vision check before moving
            PerformVisionCheck();
            
            float[] input =
            {
                visionData[0], 
                visionData[1], 
                visionData[2],
                visionData[3],
                visionData[4],
                visionData[5],
                visionData[6],
                visionData[7],
                dna.hunger / 100,
                dna.health / 100
            };
            
            
            int direction = dna.ChooseDirection(input);
        
            moveDirection = Dna.possibleDirections[direction] * dna.stepSize;
            transform.Translate(moveDirection);
        
            dna.movesTaken.Add(moveDirection);
            timer = 0f;
        }
    }

    private void PerformVisionCheck()
    {
        for (int i = 0; i < dna.visionDirections.Length; i++)
        {
            Vector2 rayOrigin = transform.position;
            Vector2 rayDirection = dna.visionDirections[i];
        
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayDirection, dna.visionRange);
        
            RaycastHit2D hit = default;
            foreach (var h in hits)
            {
                // Skip self
                if (h.collider.gameObject == gameObject)
                    continue;
                
                hit = h;
                break; 
            }
        
            if (hit.collider)
            {
                if (hit.collider.CompareTag("food"))
                {
                    visionData[i] = 1f;
                }
                else if (hit.collider.CompareTag("negative") || hit.collider.CompareTag("poisson"))
                {
                    visionData[i] = -1f;
                }
                else
                {
                    visionData[i] = 0f;
                }
            }
            else
            {
                visionData[i] = 0f;
            }
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
            dna.health -= healthDecreaseRate * Time.deltaTime;
        }

        if (dna.health <= 0)
        {
            dna.fitness -= 10;
            isActive = false;
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void GrabbedFood()
    {
        dna.hunger += 100f;
        dna.health += 100f;
        dna.fitness += 10f;

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
        dna.hunger -= 5f;
        dna.health -= 5f;
        dna.fitness -= 10;

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
        dna.health = 0f;   
        dna.fitness -= 10;
    }
    
    public void ResetAgent()
    {
        timer = 0f;  
        dna.fitness = 0;
        dna.health = dna.GetMaxHealth();
        dna.hunger = dna.GetMaxHunger();
        isActive = true;
    }

    private void OnDrawGizmos()
    {
        if (dna == null)
            return;
        
        Gizmos.color = Color.cyan;
        foreach (Vector2 dir in dna.visionDirections)
        {
            Vector2 start = transform.position;
            Vector2 end = start + dir * dna.visionRange;
            Gizmos.DrawLine(start, end);
        }
    }
}