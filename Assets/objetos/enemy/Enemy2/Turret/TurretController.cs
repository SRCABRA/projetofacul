using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretController : EnemyPai
{
    [Header("Turret Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 4f;
    private float fireTimer = 0f;

    [Header("Detection")]
    public float detectionRange = 10f;
    private Transform playerTransform;
    public LayerMask obstacleMask;

    [Header("FX")]
    public GameObject explosionEffect;

    [Header("Visão / Piscada")]
    public bool enableVisionBlink = true;
    [SerializeField] private float blinkDuration = 0.2f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private Color colorWhenSeesPlayer = Color.red;
    [SerializeField] private Color colorWhenNotSeeing = Color.green;

    private Renderer[] allRenderers;
    private Color[] originalColors;
    private bool lastStateCanSeePlayer = false;
    private bool blinking = false;

    protected override void Start()
    {
        base.Start();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Cache all renderers (self + children)
        allRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[allRenderers.Length];

        for (int i = 0; i < allRenderers.Length; i++)
        {
            if (allRenderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = allRenderers[i].material.color;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool canSee = false;

        if (distanceToPlayer <= detectionRange)
        {
            canSee = CanSeePlayer();

            if (canSee)
            {
                fireTimer += Time.deltaTime;

                if (fireTimer >= fireRate)
                {
                    Shoot();
                    fireTimer = 0f;
                }
            }
        }

        // Piscar se houve mudança de estado
        if (enableVisionBlink && canSee != lastStateCanSeePlayer && !blinking)
        {
            Color blinkColor = canSee ? colorWhenSeesPlayer : colorWhenNotSeeing;
            StartCoroutine(BlinkColor(blinkColor));
        }

        lastStateCanSeePlayer = canSee;
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.5f);
            return false;
        }

        Debug.DrawLine(transform.position, playerTransform.position, Color.green, 0.5f);
        return true;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    protected override void Drop()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        base.Drop();
    }

    private IEnumerator BlinkColor(Color blinkColor)
    {
        blinking = true;

        for (int i = 0; i < blinkCount; i++)
        {
            SetColorToAll(blinkColor);
            yield return new WaitForSeconds(blinkDuration);
            RestoreOriginalColors();
            yield return new WaitForSeconds(blinkDuration);
        }

        blinking = false;
    }

    private void SetColorToAll(Color color)
    {
        foreach (Renderer rend in allRenderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                rend.material.color = color;
            }

            if (rend.material.HasProperty("_EmissionColor"))
            {
                Color emissionColor = color * 2f; // Aumenta intensidade do brilho
                rend.material.SetColor("_EmissionColor", emissionColor);
                rend.material.EnableKeyword("_EMISSION");
            }
        }
    }


private void RestoreOriginalColors()
{
    for (int i = 0; i < allRenderers.Length; i++)
    {
        if (allRenderers[i].material.HasProperty("_Color"))
        {
            allRenderers[i].material.color = originalColors[i];
        }

        if (allRenderers[i].material.HasProperty("_EmissionColor"))
        {
            allRenderers[i].material.SetColor("_EmissionColor", Color.black);
            allRenderers[i].material.DisableKeyword("_EMISSION");
        }
    }
}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
