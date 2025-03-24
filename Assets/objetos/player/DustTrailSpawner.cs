using UnityEngine;

public class DustTrailSpawner : MonoBehaviour
{
    public GameObject dustPrefab; // Prefab da poeira
    public float spawnRate = 0.05f; // Tempo entre cada poeira gerada

    private float nextSpawnTime = 0.2f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verifica se há entrada de movimento do jogador
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // Somente gera poeira se o personagem estiver no chão e se movendo por input
        if (controller.isGrounded && isMoving && Time.time >= nextSpawnTime)
        {
            SpawnDust();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnDust()
    {
        // Obtém a posição dos pés do jogador automaticamente
        float footY = controller.bounds.min.y;

        // Define a posição da poeira com um leve random na horizontal
        Vector3 spawnPos = new Vector3(
            transform.position.x + Random.Range(-0.1f, 0.1f), // Pequena variação no X
            footY, // Sempre no chão
            transform.position.z + Random.Range(-0.1f, 0.1f)  // Pequena variação no Z
        );

        Instantiate(dustPrefab, spawnPos, Quaternion.identity);
    }
}
