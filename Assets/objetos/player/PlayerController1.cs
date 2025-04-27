using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController1 : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public bool enableMovement = true;
    public bool enableJump = true;
    public bool enableVariableJump = true;
    public bool enableAcceleratedFall = true;
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

    [Header("Pulo Variável")]
    [SerializeField] private float variableJumpRiseSpeed = 10f;
    [SerializeField] private float maxJumpTime = 0.3f;
    [SerializeField] private float jumpHoldForce = 10f;
    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Queda Acelerada")]
    [SerializeField] private float fallAcceleration = 2.0f;
    [SerializeField] private float maxFallSpeed = -30f;
    private bool isFalling = false;
    private float fallTimer = 0f;

    [Header("Efeito de Pulo")]
    [SerializeField] private GameObject jumpVFXPrefab;

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

    [Header("Invulnerabilidade")]
    [SerializeField] private float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;

    [Header("Efeito Visual de Invulnerabilidade")]
    private readonly List<Collider> ignoredEnemyColliders = new List<Collider>();
    [SerializeField] private bool enableInvulnerabilityBlink = true;
    [SerializeField] private Color blinkColor = Color.blue;
    [SerializeField] private float blinkInterval = 0.2f;

    private Renderer[] renderersToBlink;
    private Color[] originalColors;
    private Coroutine blinkCoroutine;

    void Start()
    {
        playerBuffs = GetComponent<PlayerBuffs>();
        controller = GetComponent<CharacterController>();
        MyCamera = Camera.main.transform;
        cameraOffset = MyCamera.position - transform.position;
        animator = GetComponentInChildren<Animator>();

        renderersToBlink = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderersToBlink.Length];
        for (int i = 0; i < renderersToBlink.Length; i++)
        {
            renderersToBlink[i].material = new Material(renderersToBlink[i].material);
            if (renderersToBlink[i].material.HasProperty("_Color"))
                originalColors[i] = renderersToBlink[i].material.color;
        }
    }

    void Update()
    {
        if (enableMovement)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 position = new Vector3(horizontal, 0, vertical);
            position = MyCamera.TransformDirection(position);
            position.y = 0f;

            controller.Move(position * speed * Time.deltaTime);

            if (enableAnimations)
            {
                animator.SetBool("move", position != Vector3.zero);
                animator.SetBool("idle", position == Vector3.zero);
            }

            if (position != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(position), Time.deltaTime * 10);
            }
        }

        isGrounded = Physics.CheckSphere(foot.position, 0.3f, colisaoLayer);

        if (enableAnimations)
        {
            animator.SetBool("jump", !isGrounded);
        }

        // PULO INICIAL
        if (enableJump && isGrounded && Input.GetButtonDown("Jump"))
        {
            gravity = jumpForce;
            PlayJumpVFX();
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            timeInAir = 0f;
        }

        // PULO VARIÁVEL
        if (enableJump && enableVariableJump && Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                gravity = variableJumpRiseSpeed;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }

        // STOMP
        if (enableStomp && Input.GetButtonDown("Jump"))
        {
            if (Time.time - lastJumpTime < doubleClickTime)
            {
                ActivateStompAbility();
            }
            lastJumpTime = Time.time;
        }

        // GRAVIDADE e QUEDA ACELERADA
        if (!isGrounded)
        {
            if (enableAcceleratedFall && gravity < 0)
            {
                isFalling = true;
                fallTimer += Time.deltaTime;
                gravity += -fallAcceleration * fallTimer;
                gravity = Mathf.Max(gravity, maxFallSpeed);
            }
            else
            {
                fallTimer = 0f;
                isFalling = false;
                gravity += -25f * Time.deltaTime;
            }

            timeInAir += 10 * Time.deltaTime;
        }
        else
        {
            fallTimer = 0f;
            isFalling = false;
        }

        // MOVIMENTO VERTICAL
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        if (enableCameraFollow)
        {
            Vector3 newCameraPosition = transform.position + cameraOffset;
            MyCamera.position = newCameraPosition;
        }

        if (enableMinePlacement && canPlaceMine && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("PlaceMine")))
        {
            Vector3 dropPosition = transform.position + transform.forward * 1.5f;
            mineBuffSystem.PlaceMine(dropPosition);
        }

        if (stompActivated && !isGrounded)
        {
            IgnoreAllEnemyCollisions(true);
        }
        else if (isGrounded)
        {
            IgnoreAllEnemyCollisions(false);
        }
    }

    private void PlayJumpVFX()
    {
        if (jumpVFXPrefab != null)
        {
            Instantiate(jumpVFXPrefab, foot.position, Quaternion.identity);
        }
    }

    void ActivateStompAbility()
    {
        if (Time.time - lastAbilityTime >= abilityCooldown)
        {
            stompActivated = true;
            lastAbilityTime = Time.time;
            gravity = extraGravityForce;
            StartCoroutine(ActivateTemporaryInvulnerability());
        }
    }

    private IEnumerator ActivateTemporaryInvulnerability()
    {
        SetInvulnerability(true);
        yield return new WaitForSeconds(invulnerabilityDuration);
        yield return new WaitForSeconds(1f);
        SetInvulnerability(false);
    }

    public void SetInvulnerability(bool active)
    {
        isInvulnerable = active;

        if (enableInvulnerabilityBlink)
        {
            if (active)
            {
                if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
                blinkCoroutine = StartCoroutine(BlinkEffect());
            }
            else
            {
                if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
                RestoreOriginalColors();
            }
        }

        ToggleEnemyCollision(!active);
    }

    private IEnumerator BlinkEffect()
    {
        bool toggle = false;
        while (isInvulnerable)
        {
            foreach (Renderer rend in renderersToBlink)
            {
                if (rend.material.HasProperty("_Color"))
                    rend.material.color = toggle ? blinkColor : Color.white;
            }
            toggle = !toggle;
            yield return new WaitForSeconds(blinkInterval);
        }

        RestoreOriginalColors();
    }

    private void RestoreOriginalColors()
    {
        for (int i = 0; i < renderersToBlink.Length; i++)
        {
            if (renderersToBlink[i].material.HasProperty("_Color"))
                renderersToBlink[i].material.color = originalColors[i];
        }
    }

    private void ToggleEnemyCollision(bool enable)
    {
        foreach (Collider enemyCol in ignoredEnemyColliders)
        {
            foreach (Collider playerCol in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(playerCol, enemyCol, !enable);
            }
        }

        if (!enable) ignoredEnemyColliders.Clear();
    }

    private void IgnoreAllEnemyCollisions(bool ignore)
    {
        Collider[] enemyColliders = GameObject.FindGameObjectsWithTag("enemy")
            .SelectMany(obj => obj.GetComponentsInChildren<Collider>())
            .ToArray();

        foreach (Collider enemyCol in enemyColliders)
        {
            foreach (Collider playerCol in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(playerCol, enemyCol, ignore);
            }

            if (ignore && !ignoredEnemyColliders.Contains(enemyCol))
                ignoredEnemyColliders.Add(enemyCol);
        }

        if (!ignore)
        {
            ignoredEnemyColliders.Clear();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isInvulnerable && hit.gameObject.CompareTag("enemy"))
        {
            CacheEnemyCollider(hit.collider);
            return;
        }

        if (stompActivated && hit.gameObject.CompareTag("enemy"))
        {
            Vector3 contactNormal = hit.normal;
            bool isFromAbove = Vector3.Dot(contactNormal, Vector3.up) > 0.5f;

            if (isFromAbove)
            {
                Destroy(hit.gameObject);
                Debug.Log("Inimigo pisado durante o stomp!");
                return;
            }
        }

        if (stompActivated && ((1 << hit.gameObject.layer) & colisaoLayer) != 0)
        {
            float newScale = 1f + (timeInAir * scaleIncreasePerSecond);
            Vector3 areaAttackScale = new Vector3(newScale, newScale, newScale);

            GameObject areaAttackInstance = Instantiate(AreaAttack, foot.position, Quaternion.identity);
            areaAttackInstance.transform.localScale = areaAttackScale;

            Debug.Log($"Área de ataque instanciada com escala {areaAttackScale}");

            // AQUI: tremer a câmera!
            ManualCameraShake.Instance?.Shake(0.2f, 0.3f);

            stompActivated = false;
            IgnoreAllEnemyCollisions(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInvulnerable && other.CompareTag("enemy"))
        {
            CacheEnemyCollider(other);
            return;
        }

        if (!enableBuffSystem) return;

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
            SetInvulnerability(true);
            Invoke(nameof(DisableBuffInvulnerability), 5f);
        }
    }

    private void CacheEnemyCollider(Collider enemyCol)
    {
        if (!ignoredEnemyColliders.Contains(enemyCol))
        {
            foreach (Collider playerCol in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(playerCol, enemyCol, true);
            }
            ignoredEnemyColliders.Add(enemyCol);
        }
    }

    private void DisableBuffInvulnerability()
    {
        SetInvulnerability(false);
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
