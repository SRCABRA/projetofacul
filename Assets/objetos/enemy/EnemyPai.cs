using UnityEngine;

public class EnemyPai : MonoBehaviour
{
    public float life = 10f;
    public float danoBullet = 4f;
    public float danoAreaAttack = 6f;
    public float danoBackpack = 3f;

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
        if (life <= 0)
        {
            Drop();
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack") && !isJumping)
        {
            isJumping = true;
            life -= danoAreaAttack;
            JumpEffect();
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            life -= danoBullet;
        }
        else if (collision.gameObject.CompareTag("backpack"))
        {
            life -= danoBackpack;
        }

        FlashRed();
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
