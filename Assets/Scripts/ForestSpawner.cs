using UnityEngine;
public class ForestSpawner : MonoBehaviour
{
    [Header("Prefabs")] 
    [SerializeField] private GameObject goodFood;
    [SerializeField] private GameObject badFood;

    [Header("Settings")]
    [SerializeField] private Transform foodParent;
    [SerializeField] private int amountGood;
    [SerializeField] private int amountBad;
    [SerializeField] private Vector2 xRange;
    [SerializeField] private Vector2 yRange;

    private void Start()
    {
        CreateGoodFood();
        CreateBadFood();
    }
    
    private void CreateGoodFood()
    {
        for (int i = 0; i < amountGood; i++)
        {
            var go = Instantiate(goodFood, foodParent);

            if (go.TryGetComponent<Food>(out var food))
            {
                food.Initialize(xRange, yRange);
            }
        }
    }
    private void CreateBadFood()
    {
        for (int i = 0; i < amountBad; i++)
        {
            var go = Instantiate(badFood, foodParent);

            if (go.TryGetComponent<Food>(out var food))
            {
                food.Initialize(xRange, yRange);
            }
        }
    }
}
