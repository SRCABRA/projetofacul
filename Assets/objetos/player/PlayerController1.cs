using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    // Variáveis de movimentação e física
    public float speed = 15.0f; // velocidade do personagem
    public float gravity = -10f; // gravidade normal
    public float jumpForce = 5f; // força do pulo
    private bool isGrounded; // verifica se o personagem está no chão 

    [SerializeField] private Transform foot; // ponto para verificação de colisão com o chão
    [SerializeField] private LayerMask colisaoLayer;

    private Transform MyCamera; // câmera do personagem
    private CharacterController controller; // controlador de personagem            
    public Vector3 cameraOffset; // offset da câmera em relação ao jogador

    // Variáveis do STOMP --------------------------------------------
    public float abilityActivationHeight = 5.0f; // altura mínima para ativar a habilidade
    public float extraGravityForce = -20f; // força extra para simular aumento da gravidade
    public float abilityCooldown = 5f; // tempo de recarga da habilidade

    public GameObject AreaAttack; // prefab da área de ataque

    private float lastAbilityTime = -10f; // armazena o último tempo em que a habilidade foi acionada
    private bool stompActivated = false; // variável para rastrear se a habilidade de STOMP foi ativada

    // Variáveis para detectar duplo clique
    private float lastJumpTime = 0f;
    private float doubleClickTime = 0.3f; // intervalo máximo entre cliques para ser considerado um duplo clique

    // Variáveis para calcular a escala do AreaAttack
    private float timeInAir = 0f;
    public float scaleIncreasePerSecond = 0.1f; // valor para aumentar a escala por segundo no ar

    private PlayerBuffs playerBuffs; // Referência para o sistema de buffs

    void Start()
    {
        playerBuffs = GetComponent<PlayerBuffs>(); 
        controller = GetComponent<CharacterController>();
        MyCamera = Camera.main.transform;
        cameraOffset = MyCamera.position - transform.position;
    }

    void Update()
    {
        // Captura inputs de movimentação e calcula a direção relativa à câmera
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 position = new Vector3(horizontal, 0, vertical);
        position = MyCamera.TransformDirection(position);
        position.y = 0f; // zera o eixo y

        // Movimenta o personagem
        controller.Move(position * speed * Time.deltaTime);

        // Rotaciona o personagem na direção do movimento
        if (position != Vector3.zero){
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10);
        }
        
        // Verifica se o personagem está no chão
        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer);

        // Código do pulo
        if (Input.GetButtonDown("Jump") && isGrounded) // Se o personagem estiver no chão e o botão de pulo for pressionado
        {
            gravity = jumpForce;
            timeInAir = 0f; // Reseta o tempo no ar ao pular
        }

        // Verifica se o jogador pressionou o botão de pulo duas vezes rapidamente
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.time - lastJumpTime < doubleClickTime)
            {
                // Ativa a habilidade de STOMP se o jogador pressionar o botão de pulo duas vezes rapidamente
                ActivateStompAbility();
            }
            lastJumpTime = Time.time;
        }

        // Atualiza a gravidade: aplica aceleração para simular efeito de queda
        if (gravity > -10f){
            gravity += -25f * Time.deltaTime;
        }
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        // Atualiza o tempo no ar se o personagem não estiver no chão
        if (!isGrounded){
            timeInAir += 10 * Time.deltaTime;
        }

        // Atualiza a posição da câmera para seguir o jogador
        Vector3 newCameraPosition = transform.position + cameraOffset;
        MyCamera.position = newCameraPosition;
    }

    void ActivateStompAbility()
    {
        if (Time.time - lastAbilityTime >= abilityCooldown)
        {
            // Ativa a habilidade de STOMP
            stompActivated = true;
            lastAbilityTime = Time.time;

            // Adiciona a força extra da gravidade
            gravity = extraGravityForce;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifica se o jogador colidiu com o chão após usar a habilidade de STOMP
        if (stompActivated && ((1 << hit.gameObject.layer) & colisaoLayer) != 0)
        {
            // Calcula a nova escala do AreaAttack com base no tempo no ar
            float newScale = 1f + (timeInAir * scaleIncreasePerSecond);
            Vector3 areaAttackScale = new Vector3(newScale, newScale, newScale);

            // Instancia a área de ataque na posição do foot com a nova escala
            GameObject areaAttackInstance = Instantiate(AreaAttack, foot.position, Quaternion.identity);
            areaAttackInstance.transform.localScale = areaAttackScale;

            stompActivated = false; // Reseta a variável stompActivated
        }

        // Verifica se o jogador colidiu com um inimigo
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player colidiu com inimigo!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("consumable1")) // Buff de velocidade
        {
            Destroy(other.gameObject); // Destroi o consumível
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Speed, 5f, 5.0f); // Aplica o buff de velocidade
            }
        }
        else if (other.gameObject.CompareTag("consumable2")) // Buff de dano
        {
            Destroy(other.gameObject);
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Damage, 5f, 5f); // Aplica o buff de dano
            }
        }
        else if (other.gameObject.CompareTag("consumable4")) // Buff de invulnerabilidade
        {
            Destroy(other.gameObject);
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Invulnerability, 5f); // Jogador fica invulnerável por 5 segundos
            }
        }
    }

    void OnDrawGizmos() // desenha uma esfera para representar o pé do jogador
    {
        Gizmos.color = Color.red; // cor vermelha
        Gizmos.DrawWireSphere(foot.position, 0.3f); // desenha uma esfera na posição do foot com raio 0.3
    }
}
