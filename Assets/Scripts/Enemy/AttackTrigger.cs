using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER terkena serangan!");

            PlayerMovement hp = other.GetComponent<PlayerMovement>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }
    }

}
