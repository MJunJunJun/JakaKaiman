using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public EnemyController enemyController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.SetPlayerDetected(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.SetPlayerLost();
        }
    }
}
