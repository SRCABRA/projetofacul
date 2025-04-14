using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public bool enableMovement = true;
    public bool enableJump = true;
    public bool enableStomp = true;
    public bool enableCameraFollow = true;
    public bool enableMinePlacement = true;
    public bool enableBuffSystem = true;
    public bool enableAnimations = true;

    [Header("Movimentação")]
    public float speed = 15.0f;
    public float gravity = -10f;
    public float jumpForce = 5f;
    
    private bool isGrounded;

    [Header("Configurações de Detecção")]
    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask colisaoLayer;

    private Transform MyCamera;
    private CharacterController controller;
    [SerializeField] private Vector3 cameraOffset;

    [Header("Habilidade Stomp")]
    [SerializeField] private float abilityActivationHeight = 5.0f;
    [SerializeField] private float extraGravityForce = -20f;
    [SerializeField] private float abilityCooldown = 5f;
    [SerializeField] private GameObject AreaAttack;
    private float lastAbilityTime = -10f;
    private bool stompActivated = false;

    private float lastJumpTime = 0f;
    private float doubleClickTime = 0.3f;

    private float timeInAir = 0f;
    [SerializeField] private float scaleIncreasePerSecond = 0.1f;

    private PlayerBuffs playerBuffs;
    private Animator animator;

    private bool canPlaceMine = false;
    private PlayerBuffs mineBuffSystem;

    void Start()
    {
        playerBuffs = GetComponent<PlayerBuffs>();
        controller = GetComponent<CharacterController>();
        MyCamera = Camera.main.transform;
        cameraOffset = MyCamera.position - transform.position;
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (enableMovement)
        {
            // Captura inputs de movimentação
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 position = new Vector3(horizontal, 0, vertical);
            position = MyCamera.TransformDirection(position);
            position.y = 0f;

            // Movimenta o personagem
            controller.Move(position * speed * Time.deltaTime);

            if (enableAnimations)
            {
                // Atualiza animações
                animator.SetBool("move", position != Vector3.zero);
                animator.SetBool("idle", position == Vector3.zero);
            }

            // Rotaciona o personagem na direção do movimento
            if (position != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10);
            }
        }

        // Verifica se o personagem está no chão
        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer);
        
        if (enableAnimations)
        {
            animator.SetBool("jump", !isGrounded);
        }

        // Código do pulo
        if (enableJump && Input.GetButtonDown("Jump") && isGrounded)
        {
            gravity = jumpForce;
            timeInAir = 0f;
        }

        // Verifica duplo clique para ativar a habilidade STOMP
        if (enableStomp && Input.GetButtonDown("Jump"))
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
        if (enableCameraFollow)
        {
            Vector3 newCameraPosition = transform.position + cameraOffset;
            MyCamera.position = newCameraPosition;
        }

        // **Lógica para colocar a mina**
        if (enableMinePlacement && canPlaceMine && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("PlaceMine")))
        {
            Vector3 dropPosition = transform.position + transform.forward * 1.5f; // Coloca a mina na frente do jogador
            mineBuffSystem.PlaceMine(dropPosition);
        }
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
        if (stompActivated && ((1 << hit.gameObject.layer) & colisaoLayer) != 0)
        {
            float newScale = 1f + (timeInAir * scaleIncreasePerSecond);
            Vector3 areaAttackScale = new Vector3(newScale, newScale, newScale);

            GameObject areaAttackInstance = Instantiate(AreaAttack, foot.position, Quaternion.identity);
            areaAttackInstance.transform.localScale = areaAttackScale;

            stompActivated = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enableBuffSystem) return; // Desativa buffs se estiver desativado no Inspector

        if (other.gameObject.CompareTag("consumable1"))
        {
            Destroy(other.gameObject);
            playerBuffs?.ApplyBuff(BuffType.Speed, 5f, 5.0f);
        }
        else if (other.gameObject.CompareTag("consumable2"))
        {
            Destroy(other.gameObject);
            playerBuffs?.ApplyBuff(BuffType.Damage, 5f, 5f);
        }
        else if (other.gameObject.CompareTag("consumable4"))
        {
            Destroy(other.gameObject);
            playerBuffs?.ApplyBuff(BuffType.Invulnerability, 5f);
        }
    }

    public void EnableMinePlacement(PlayerBuffs buffSystem)
    {
        if (!enableMinePlacement) return;

        canPlaceMine = true;
        mineBuffSystem = buffSystem;
        Debug.Log("Você pode colocar minas! Pressione 'E' para colocar.");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(foot.position, 0.3f);
    }
}
