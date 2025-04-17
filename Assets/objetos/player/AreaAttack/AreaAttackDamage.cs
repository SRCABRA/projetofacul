using UnityEngine;

public class AreaAttackDamage : MonoBehaviour
{
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("enemy"))
            {
                Destroy(other.gameObject); // ou aplicar dano, etc.
                Debug.Log("Inimigo atingido pelo stomp!");
            }
        }
    }


