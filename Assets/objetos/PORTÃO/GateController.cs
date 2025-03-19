using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public int requiredEnemiesDefeated = 5; // Número necessário de inimigos mortos
    public float descentSpeed = 2f; // Velocidade de descida da barreira
    public float descentHeight = 3f; // Distância que a barreira desce
    public BarrierController previousBarrier; // Referência ao portão anterior
    
    private int totalEnemies = -1; // Inicializado como -1 para indicar que ainda não foi definido
    private bool isOpening = false;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition - new Vector3(0, descentHeight, 0);
    }

    void Update()
    {
        if (previousBarrier != null && !previousBarrier.isOpening)
        {
            return; // Aguarda o portão anterior abrir antes de começar a contar inimigos
        }

        if (totalEnemies == -1)
        {
            totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length; // Define o total apenas quando o portão anterior abrir
        }

        int remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int enemiesDefeated = totalEnemies - remainingEnemies; // Calcula quantos foram destruídos

        if (enemiesDefeated >= requiredEnemiesDefeated)
        {
            isOpening = true;
        }

        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * descentSpeed);
        }
    }
}
