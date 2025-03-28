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
    protected Renderer enemyRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    private bool isJumping = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
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
        if (enemyRenderer != null && !isFlashing)
        {
            isFlashing = true;
            enemyRenderer.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    protected void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
        isFlashing = false;
    }

    protected void JumpEffect()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
        }
    }

    protected virtual void Drop()
    {
        // Implementação vazia, caso um inimigo filho precise sobrescrever esse método
    }
}
