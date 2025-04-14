using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour
{
    [Header("Configurações de Buffs")]
    public bool enableSpeedBuff = true;
    public bool enableDamageBuff = true;
    public bool enableInvulnerabilityBuff = true;
    public bool enableMineSystem = true;
    public bool enableBuffExpiration = true;

    [Header("Referências")]
    [SerializeField] private GameObject consumable3buffPrefab;
    [SerializeField] private GameObject minePrefab;

    private PlayerController1 playerController1;
    private EnemyPai enemyController;
    private LifeController lifeController;

    private float originalSpeed;
    private float originalDamage;

    private float activeSpeedBonus = 0f;
    private float activeDamageMultiplier = 1f;
    public float CurrentDamage { get { return originalDamage * activeDamageMultiplier; } }

    private List<Buff> activeBuffs = new List<Buff>();

    [Header("Configurações de Minas")]
    [SerializeField] private int usesmines = 2;
    private int mineUses = 0; // Agora começa em 0 e acumula

    void Start()
    {
        playerController1 = GetComponent<PlayerController1>();
        lifeController = GetComponent<LifeController>();
        enemyController = FindFirstObjectByType<EnemyPai>();

        if (playerController1 != null)
        {
            originalSpeed = playerController1.speed;
        }
        if (enemyController != null)
        {
            originalDamage = enemyController.danoBullet;
        }
    }

    void Update()
    {
        if (enableBuffExpiration)
        {
            float currentTime = Time.time;
            activeBuffs.RemoveAll(buff => buff.ExpireTime <= currentTime);
        }

        float speedBuff = 0f;
        float damageBuffMultiplier = 1f;

        foreach (Buff buff in activeBuffs)
        {
            if (buff.Type == BuffType.Speed) speedBuff = activeSpeedBonus;
            if (buff.Type == BuffType.Damage) damageBuffMultiplier = activeDamageMultiplier;
        }

        if (enableSpeedBuff && playerController1 != null)
        {
            playerController1.speed = originalSpeed + speedBuff;
        }

        if (enableDamageBuff && enemyController != null)
        {
            enemyController.danoBullet = originalDamage * damageBuffMultiplier;
        }
    }

    public void ApplyBuff(BuffType type, float duration, float value = 0)
    {
        if (!enableSpeedBuff && type == BuffType.Speed) return;
        if (!enableDamageBuff && type == BuffType.Damage) return;
        if (!enableInvulnerabilityBuff && type == BuffType.Invulnerability) return;

        float expireTime = Time.time + duration;
        activeBuffs.Add(new Buff(type, expireTime));

        if (type == BuffType.Speed && playerController1 != null)
        {
            activeSpeedBonus = value;
            playerController1.speed += activeSpeedBonus;
            Debug.Log($"Buff de Velocidade Ativado! Nova velocidade: {playerController1.speed}");
        }
        else if (type == BuffType.Damage && enemyController != null)
        {
            activeDamageMultiplier = value;
            enemyController.danoBullet = originalDamage * activeDamageMultiplier;
            Debug.Log($"Buff de Dano Ativado! Novo dano: {enemyController.danoBullet}");
        }
        else if (type == BuffType.Invulnerability && lifeController != null)
        {
            lifeController.ActivateInvulnerability(duration);
            Debug.Log("Jogador ficou invulnerável!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("consumable3"))
        {
            Vector3 spawnPosition = other.transform.position;
            Destroy(other.gameObject);
            Instantiate(consumable3buffPrefab, spawnPosition, Quaternion.identity);
        }
        else if (enableInvulnerabilityBuff && other.gameObject.CompareTag("consumable4"))
        {
            Destroy(other.gameObject);
            ApplyBuff(BuffType.Invulnerability, 5f);
        }
        else if (enableMineSystem && other.gameObject.CompareTag("consumable5"))
        {
            Destroy(other.gameObject);
            mineUses += usesmines;
            ActivateMinePlacement();
            Debug.Log($"Pegou um Consumable5! Minas disponíveis: {mineUses}");
        }
    }

    void ActivateMinePlacement()
    {
        if (!enableMineSystem) return;

        Debug.Log("Pressione 'E' para colocar uma mina.");
        playerController1.EnableMinePlacement(this);
    }

    public void PlaceMine(Vector3 position)
    {
        if (!enableMineSystem) return;

        if (mineUses > 0)
        {
            Instantiate(minePrefab, position, Quaternion.identity);
            mineUses--;
            Debug.Log($"Mina colocada! Restantes: {mineUses}");

            if (mineUses <= 0)
            {
                Debug.Log("Acabaram suas minas! Pegue mais consumables5 para ganhar mais.");
            }
        }
    }
}

public enum BuffType { Speed, Damage, Invulnerability }

public class Buff
{
    public BuffType Type { get; private set; }
    public float ExpireTime { get; private set; }

    public Buff(BuffType type, float expireTime)
    {
        Type = type;
        ExpireTime = expireTime;
    }
}
