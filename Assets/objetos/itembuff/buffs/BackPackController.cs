using UnityEngine;

public class BackPackController : MonoBehaviour
{
    public string playerTag = "Player"; // Tag do objeto que será seguido
    public float orbitRadius = 2f;  // Raio da órbita
    public float orbitSpeed = 50f;  // Velocidade da órbita
    public float verticalSpeed = 1f; // Velocidade do movimento vertical (espiral)
    public float verticalRange = 1.5f; // Altura máxima da espiral
    public Vector3 orbitAxis = Vector3.up; // Eixo da órbita (padrão: eixo Y)

    private Transform player; // Referência ao Transform do player
    private float angle; // Ângulo da órbita

    void Start()
    {
        // Encontra o objeto com a tag "Player" automaticamente
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("BackPackController: Nenhum objeto com a tag '" + playerTag + "' foi encontrado!");
        }
    }

    void LateUpdate() // LateUpdate ajuda a evitar tremores ao mover o objeto após o Player já ter atualizado sua posição
    {
        if (player == null) return; // Se o Player não foi encontrado, não faz nada

        // Atualiza o ângulo baseado no tempo e velocidade
        angle += orbitSpeed * Time.deltaTime;

        // Calcula o deslocamento da órbita usando seno e cosseno para evitar cálculos de rotação inconsistentes
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * orbitRadius;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * orbitRadius;

        // Adiciona variação na altura para criar a espiral
        float heightOffset = Mathf.Sin(angle * Mathf.Deg2Rad * verticalSpeed) * verticalRange;

        // Define a nova posição do item
        transform.position = player.position + new Vector3(x, heightOffset, z);
    }
}
