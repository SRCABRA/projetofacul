using UnityEngine;

public class PizzaDeliveryPoint : MonoBehaviour
{
    [Header("Ponto de entrega das pizzas")]
    [SerializeField] private Transform deliveryPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeController life = other.GetComponent<LifeController>();
        if (life == null) return;

        int pizzasEntregues = 0;

        foreach (Transform child in other.transform)
        {
            if (child.CompareTag("PizzaBox"))
            {
                // Move a pizza para a posição de entrega
                child.SetParent(null);
                child.position = deliveryPosition.position + new Vector3(0, 0.5f * pizzasEntregues, 0);
                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                pizzasEntregues++;
            }
        }

        life.MarkPizzaDelivered(); // Impede a morte por falta de pizza
        Debug.Log($"Pizzas entregues com sucesso: {pizzasEntregues}");
    }
}
