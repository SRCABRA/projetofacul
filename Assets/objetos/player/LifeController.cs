using UnityEngine;
using System.Collections;

public class LifeController : MonoBehaviour
{
    public int pizzabox = 3;
    public GameObject Player;
    public GameObject BoxPrefab; // Prefab da caixa de pizza, objeto a ser instanciado
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 1.0f; // Duração da invulnerabilidade em segundos
    private int boxPrefabCount = 0; // Contador de BoxPrefab instanciados

    void Start()
    {
        // Inicializa o contador de BoxPrefab na cena
        boxPrefabCount = GameObject.FindGameObjectsWithTag("BoxPizzaPrefab").Length;
    }

    void Update()
    {
        // Atualiza o contador de BoxPrefab na cena
        boxPrefabCount = GameObject.FindGameObjectsWithTag("BoxPizzaPrefab").Length;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvulnerable)
        {
            LosePizzaBox();
        }
        else if (collision.gameObject.CompareTag("BoxPizzaPrefab"))
        {
            GainPizzaBox(collision.gameObject);
        }
    }

    void LosePizzaBox()
    {
        if (pizzabox > 0)
        {
            pizzabox--;
            // Torna uma das caixas de pizza invisível
            Transform[] pizzaBoxes = Player.GetComponentsInChildren<Transform>(true);
            foreach (Transform box in pizzaBoxes)
            {
                if (box.gameObject.CompareTag("PizzaBox") && box.gameObject.activeSelf)
                {
                    box.gameObject.SetActive(false);
                    break;
                }
            }

            // Instancia uma BoxPrefab na cena
            if (boxPrefabCount < 3 - pizzabox)
            {
                Instantiate(BoxPrefab, Player.transform.position, Quaternion.identity);
                boxPrefabCount++;
            }

            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    void GainPizzaBox(GameObject boxPizzaPrefab)
    {
        if (pizzabox < 3)
        {
            pizzabox++;
            // Torna uma das caixas de pizza visível
            Transform[] pizzaBoxes = Player.GetComponentsInChildren<Transform>(true);
            foreach (Transform box in pizzaBoxes)
            {
                if (box.gameObject.CompareTag("PizzaBox") && !box.gameObject.activeSelf)
                {
                    box.gameObject.SetActive(true);
                    break;
                }
            }

            Destroy(boxPizzaPrefab);
            boxPrefabCount--;
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
}