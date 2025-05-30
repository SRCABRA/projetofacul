using UnityEngine;

public class PizzaDeliveryPoint : MonoBehaviour
{
    [Header("Ponto de entrega das pizzas")]
    [SerializeField] private Transform deliveryPosition;

    public bool JaEntregou = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LifeController life = other.GetComponent<LifeController>();
        if (life == null) return;

        int pizzasEntregues = 0;

        // Corrige o problema de modificar a hierarquia durante a iteração
        Transform[] filhos = other.GetComponentsInChildren<Transform>();

        foreach (Transform child in filhos)
        {
            if (child.parent == other.transform && child.CompareTag("PizzaBox") && child != life.pizzaGhost)
            {
                child.SetParent(null);
                child.position = deliveryPosition.position + new Vector3(0, 0.5f * pizzasEntregues, 0);

                Rigidbody rb = child.GetComponent<Rigidbody>();
                if (rb != null) Destroy(rb);

                pizzasEntregues++;
                JaEntregou = true;
            }
        }

        life.MarkPizzaDelivered(); // Evita game over por falta de pizza

        Debug.Log($"Pizzas entregues com sucesso: {pizzasEntregues}");
    }
}
