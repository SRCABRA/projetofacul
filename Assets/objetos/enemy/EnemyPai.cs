using UnityEngine;

public class EnemyPai : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public bool enableLifeSystem = true;
    public bool enableDamage = true;
    public bool enableFlashEffect = true;
    public bool enableJumpEffect = true;

    [Header("Atributos do Inimigo")]
    [SerializeField] public float life = 10f;
    [SerializeField] public float danoBullet = 4f;
    [SerializeField] public float danoAreaAttack = 6f;
    [SerializeField] public float danoBackpack = 3f;

    protected Rigidbody rb;
    protected Renderer[] renderers;
    private Color[] originalColors;
    private bool isFlashing = false;
    private bool isJumping = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Pega todos os renderers do inimigo e seus filhos
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        // Armazena a cor original de cada renderer
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    protected virtual void Update()
    {
        if (enableLifeSystem && life <= 0)
        {
            Debug.Log("Torreta destruída");
            Drop();
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (!enableDamage) return;

        if (collision.gameObject.CompareTag("AreaAttack"))
        {
            Debug.Log("COLIDIU COM A ÁREA DE ATAQUE");
        }

        if (collision.gameObject.CompareTag("AreaAttack") && !isJumping)
        {
            isJumping = true;
            life -= danoAreaAttack;
            Debug.Log($"Dano de área recebido! Vida restante: {life}");
            if (enableJumpEffect) JumpEffect();
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            life -= danoBullet;
            Debug.Log($"Atingido por Bullet! Vida restante: {life}");
        }
        else if (collision.gameObject.CompareTag("backpack"))
        {
            life -= danoBackpack;
            Debug.Log($"Atingido por Backpack! Vida restante: {life}");
        }

        if (enableFlashEffect) FlashRed();
    }

    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack"))
        {
            isJumping = false;
        }
    }

    protected void FlashRed()
    {
        if (!isFlashing)
        {
            isFlashing = true;

            // Define a cor vermelha em todos os renderers
            foreach (var rend in renderers)
            {
                rend.material.color = Color.red;
            }

            Invoke(nameof(ResetColor), 0.2f); // Volta à cor original após 0.2 segundos
        }
    }

    protected void ResetColor()
    {
        // Restaura a cor original de cada renderer
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = originalColors[i];
        }

        isFlashing = false;
    }

    protected void JumpEffect()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Corrigido de linearVelocity para velocity
            rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
        }
    }

    protected virtual void Drop()
    {
        // Implementação vazia para ser sobrescrita pelos filhos
    }
}
