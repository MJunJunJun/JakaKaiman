using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    public EnemyController enemyController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.SetInAttackZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController.SetInAttackZone(false);
        }
    }
}
