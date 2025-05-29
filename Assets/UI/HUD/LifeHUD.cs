using UnityEngine;

public class LifeHUD : MonoBehaviour
{
    [Header("Prefabs das imagens de vida")]
    public GameObject life3Prefab;
    public GameObject life2Prefab;
    public GameObject life1Prefab;
    public GameObject life0Prefab;

    private GameObject currentHUD;
    private LifeController lifeController;

    private int lastLife = -1;

    void Start()
    {
        lifeController = FindFirstObjectByType<LifeController>();
        UpdateHUD(CountRealPizzaBoxes());
    }

    void Update()
    {
        int currentLife = CountRealPizzaBoxes();
        if (currentLife != lastLife)
        {
            UpdateHUD(currentLife);
        }
    }

    void UpdateHUD(int lifeCount)
    {
        GameObject nextHUD = null;
        switch (lifeCount)
        {
            case 3:
                nextHUD = life3Prefab;
                break;
            case 2:
                nextHUD = life2Prefab;
                break;
            case 1:
                nextHUD = life1Prefab;
                break;
            default:
                nextHUD = life0Prefab;
                break;
        }

        if (currentHUD != null)
            currentHUD.SetActive(false);

        currentHUD = nextHUD;

        if (currentHUD != null)
            currentHUD.SetActive(true);

        lastLife = lifeCount;
    }

    int CountRealPizzaBoxes()
    {
        if (lifeController == null) return 0;

        int count = 0;
        foreach (Transform child in lifeController.transform)
        {
            if (child.CompareTag("PizzaBox") && child != lifeController.pizzaGhost)
            {
                count++;
            }
        }
        return count;
    }
}
