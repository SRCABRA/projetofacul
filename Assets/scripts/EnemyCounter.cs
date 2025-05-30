using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EnemyCounter : MonoBehaviour
{
    [Header("Referência ao Texto")]
    [SerializeField] private TextMeshProUGUI enemyText;

    private List<GameObject> originalEnemies = new List<GameObject>();

    void Start()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            originalEnemies.Add(enemy);
        }

        UpdateEnemyText();
    }

    void Update()
    {
        originalEnemies.RemoveAll(enemy => enemy == null);
        UpdateEnemyText();
    }

    void UpdateEnemyText()
    {
        if (enemyText != null)
        {
            enemyText.text = originalEnemies.Count.ToString();
        }
    }
}
