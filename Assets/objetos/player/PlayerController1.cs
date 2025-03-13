using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    // Variáveis de movimentação e física
    public float speed = 15.0f;
    public float gravity = -10f;
    public float jumpForce = 5f;
    private bool isGrounded;

    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask colisaoLayer;

    private Transform MyCamera;
    private CharacterController controller;
    public Vector3 cameraOffset;

    // Variáveis do STOMP
    public float abilityActivationHeight = 5.0f;
    public float extraGravityForce = -20f;
    public float abilityCooldown = 5f;
    public GameObject AreaAttack;
    private float lastAbilityTime = -10f;
    private bool stompActivated = false;

    // Detecção de duplo clique
    private float lastJumpTime = 0f;
    private float doubleClickTime = 0.3f;

    // Cálculo da escala do AreaAttack
    private float timeInAir = 0f;
    public float scaleIncreasePerSecond = 0.1f;

    private PlayerBuffs playerBuffs;

    // Referência para o Animator dentro do objeto filho "Idle"
    private Animator animator;

    void Start()
    {
        playerBuffs = GetComponent<PlayerBuffs>();
        controller = GetComponent<CharacterController>();
        MyCamera = Camera.main.transform;
        cameraOffset = MyCamera.position - transform.position;

        // Procura o Animator no objeto filho "Idle"
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator não encontrado! Certifique-se de que o objeto 'Idle' dentro do Player tem um Animator.");
        }
    }

    void Update()
    {
        // Captura inputs de movimentação
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 position = new Vector3(horizontal, 0, vertical);
        position = MyCamera.TransformDirection(position);
        position.y = 0f;

        // Movimenta o personagem
        controller.Move(position * speed * Time.deltaTime);

        // Verifica a velocidade atual do jogador
        float currentSpeed = position.magnitude;

        // Atualiza o Animator com a velocidade para alternar entre Idle e Running
        animator.SetBool("move", position != Vector3.zero);
        animator.SetBool("idle", position == Vector3.zero);
        

        // Rotaciona o personagem na direção do movimento
        if (position != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10);
        }

        // Verifica se o personagem está no chão
        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer);
        animator.SetBool("jump", !isGrounded);

        // Código do pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            gravity = jumpForce;
            timeInAir = 0f;
        }

        // Verifica duplo clique para ativar a habilidade STOMP
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.time - lastJumpTime < doubleClickTime)
            {
                ActivateStompAbility();
            }
            lastJumpTime = Time.time;
        }

        // Atualiza a gravidade
        if (gravity > -10f)
        {
            gravity += -25f * Time.deltaTime;
        }
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        // Atualiza o tempo no ar se o personagem não estiver no chão
        if (!isGrounded)
        {
            timeInAir += 10 * Time.deltaTime;
        }

        // Atualiza a posição da câmera
        Vector3 newCameraPosition = transform.position + cameraOffset;
        MyCamera.position = newCameraPosition;
    }

    void ActivateStompAbility()
    {
        if (Time.time - lastAbilityTime >= abilityCooldown)
        {
            stompActivated = true;
            lastAbilityTime = Time.time;
            gravity = extraGravityForce;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifica se o jogador colidiu com o chão após usar o STOMP
        if (stompActivated && ((1 << hit.gameObject.layer) & colisaoLayer) != 0)
        {
            float newScale = 1f + (timeInAir * scaleIncreasePerSecond);
            Vector3 areaAttackScale = new Vector3(newScale, newScale, newScale);

            GameObject areaAttackInstance = Instantiate(AreaAttack, foot.position, Quaternion.identity);
            areaAttackInstance.transform.localScale = areaAttackScale;

            stompActivated = false;
        }

        // Verifica se o jogador colidiu com um inimigo
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player colidiu com inimigo!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("consumable1"))
        {
            Destroy(other.gameObject);
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Speed, 5f, 5.0f);
            }
        }
        else if (other.gameObject.CompareTag("consumable2"))
        {
            Destroy(other.gameObject);
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Damage, 5f, 5f);
            }
        }
        else if (other.gameObject.CompareTag("consumable4"))
        {
            Destroy(other.gameObject);
            if (playerBuffs != null)
            {
                playerBuffs.ApplyBuff(BuffType.Invulnerability, 5f);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(foot.position, 0.3f);
    }
}
    