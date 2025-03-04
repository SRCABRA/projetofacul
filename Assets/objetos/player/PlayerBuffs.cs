using UnityEngine;
using System.Collections.Generic;

public class PlayerBuffs : MonoBehaviour
{
    private PlayerController1 playerController1;
    private EnemyController enemyController;

    private float originalSpeed;
    private float originalDamage;

    private List<Buff> activeBuffs = new List<Buff>();

    void Start()
    {
        playerController1 = GetComponent<PlayerController1>();
        enemyController = FindFirstObjectByType<EnemyController>();

        if (playerController1 != null)
        {
            originalSpeed = playerController1.speed;
        }
        if (enemyController != null)
        {
            originalDamage = enemyController.danobulletbase;
        }
    }

    void Update()
    {
            float currentTime = Time.time;
            activeBuffs.RemoveAll(buff => buff.ExpireTime <= currentTime);

            // Resetando buffs que expiraram
            if (!activeBuffs.Exists(b => b.Type == BuffType.Speed) && playerController1 != null)
            {
                playerController1.speed = originalSpeed; // Restaura a velocidade original
            }

            if (!activeBuffs.Exists(b => b.Type == BuffType.Damage) && enemyController != null)
            {
                enemyController.danobulletbase = originalDamage; // Restaura o dano original
            }
    }

    public void ApplyBuff(BuffType type, float duration, float value)
    {
        float expireTime = Time.time + duration;
        activeBuffs.Add(new Buff(type, expireTime));

        if (type == BuffType.Speed && playerController1 != null) 
        {
            playerController1.speed = originalSpeed + value; // Adiciona a velocidade extra corretamente
        }

        if (type == BuffType.Damage && enemyController != null)  
        {
            enemyController.danobulletbase = originalDamage * 2; // Garante que apenas o dano é dobrado
            Debug.Log("Dano do inimigo dobrado!");
        }
    }
}

public enum BuffType { Speed, Damage }

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
