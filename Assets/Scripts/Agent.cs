using UnityEngine;

public class Agent : MonoBehaviour
{
    private Dna _dna;
    private int _moveIndex = 0;
    private bool _isActive = true;
    
    private void Start()    
    {
        _dna = GetComponent<Dna>();
    }

    private void Update()
    {
        if (!_isActive) return;
        
        if (_moveIndex < _dna.directions.Length)
        {
            transform.Translate(_dna.directions[_moveIndex] * (_dna.stepSize * Time.deltaTime));
            _moveIndex++;
        }
        else
        {
            _isActive = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("food"))
        {
            _dna.fitness += 10f;
        } else if (other.CompareTag("negative"))
        {
            _dna.fitness -= 10f;
        }
    }

    public void ResetAgent()
    {
        _moveIndex = 0;
        _isActive = true;
        _dna.fitness = 0;
    }
}
