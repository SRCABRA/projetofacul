using UnityEngine;
using System.Collections;

public class LifeController : MonoBehaviour
{
    public int pizzabox = 3;
    public GameObject Player;
    public GameObject BoxPrefab; // Prefab do novo objeto a ser instanciado
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 1.0f; // Duração da invulnerabilidade em segundos
    // Adicione uma variável para contar o número de BoxPrefab instanciados
    private int boxPrefabCount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
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
                if (box.CompareTag("PizzaBox") && box.gameObject.activeSelf)
                {
                    box.gameObject.SetActive(false);
                    break;
                }
            }

            // Verifica se o número de BoxPrefab instanciados é menor que 3
            if (boxPrefabCount < 3)
            {
                // Instancia um novo objeto BoxPrefab
                Instantiate(BoxPrefab, Player.transform.position, Quaternion.identity);
                boxPrefabCount++;
            }

            // Ativa a invulnerabilidade
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    void GainPizzaBox(GameObject boxPizzaPrefab)
    {
        // Verifica se o número de caixas de pizza visíveis é menor que 3
        Transform[] pizzaBoxes = Player.GetComponentsInChildren<Transform>(true);
        int visiblePizzaBoxes = 0;
        foreach (Transform box in pizzaBoxes)
        {
            if (box.CompareTag("PizzaBox") && box.gameObject.activeSelf)
            {
                visiblePizzaBoxes++;
            }
        }

        if (visiblePizzaBoxes < 3)
        {
            // Destrói o objeto BoxPizzaPrefab
            Destroy(boxPizzaPrefab);
            boxPrefabCount--;

            // Torna uma das caixas de pizza invisíveis visível
            bool boxActivated = false;
            foreach (Transform box in pizzaBoxes)
            {
                if (box.CompareTag("PizzaBox") && !box.gameObject.activeSelf)
                {
                    box.gameObject.SetActive(true);
                    boxActivated = true;
                    break;
                }
            }

            // Incrementa o contador de caixas de pizza apenas se uma caixa foi ativada
            if (boxActivated)
            {
                visiblePizzaBoxes++;
            }
        }

        // Atualiza o contador global de caixas de pizza visíveis
        pizzabox = visiblePizzaBoxes;
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
}