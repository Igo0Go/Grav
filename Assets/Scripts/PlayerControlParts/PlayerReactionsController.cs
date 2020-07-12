using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReactionsController : PlayerControllerBlueprint
{
    public bool Alive { get; set; }
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health <= 0)
                Death();
        }
    }
    private float _health;

    private LootPointScript currentLootPoint;
    private Collider currentMoveTransformCol;

    private GravityThrowerScript Gun => PlayerStateController.gravityThrower;
    private PlayerSceneManagementController PlayerSceneManagementController => PlayerStateController.playerSceneManagementController;
    private StatusPack StatusPack => PlayerStateController.statusPack;

    public event Action<string, bool> ActionTipMessageActivated;
    public event Action<float> HealthChanged;
    public event Action<bool> VisualizeDeathEvent;
    public event Action DeathEvent;
    public event Action FallInWaterEvent;

    protected override void SetReferences(PlayerStateController playerState)
    {
        playerState.playerSceneManagementController.SaveLocationEvent += OnSaveLocationReactor;
        playerState.playerSceneManagementController.SetMaxHealthEvent += SetMaxHealth;
        playerState.playerInputController.UsingInputEvent += UseLootPoint;
        
        

        Health = 100;
        Alive = true;
    }

    public void GetDamage(int damage)
    {
        if (Alive)
            Health -= damage;
    }
    public void Death()
    {
        if (Alive)
        {
            Alive = false;
            VisualizeDeathEvent?.Invoke(true);
            DeathEvent?.Invoke();
        }
    }

    private void OnSaveLocationReactor() => Alive = true;
    private void SetMaxHealth()
    {
        Alive = true;
        Health = 100;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            Death();
            return;
        }
        else if (other.CompareTag("Water"))
        {
            FallInWaterEvent?.Invoke();
            Invoke("Death", 1);
            return;
        }
        else if (other.CompareTag("Loot"))
        {
            LootItem loot = other.GetComponent<LootItem>();
            if (loot.opportunityToSuffice)
            {
                loot.SetTarget(PlayerStateController);
            }
            return;
        }
        else if (other.CompareTag("CheckPoint"))
        {
            if (PlayerStateController.savePoint != other.GetComponent<SavePoint>())
            {
                PlayerStateController.savePoint = other.GetComponent<SavePoint>();
                PlayerStateController.savePoint.Save(transform);
                PlayerSceneManagementController.Save(PlayerStateController.savePoint.transform);
            }
            return;
        }
        else if (other.CompareTag("SceneLoader"))
        {
            PlayerSceneManagementController.LoadNextScene();
        }
        else if (other.CompareTag("LootPoint"))
        {
            currentLootPoint = other.GetComponent<LootPointScript>();
            if(StatusPack.currentMoneyCount >= currentLootPoint.cost)
                ActionTipMessageActivated?.Invoke(currentLootPoint.tipText, true);
            else
                ActionTipMessageActivated?.Invoke("Не хватает монет для покупки", false);
            return;
        }
        else if (other.CompareTag("MoveTransform"))
        {
            currentMoveTransformCol = other;
            Mytransform.parent = currentMoveTransformCol.transform;
            return;
        }
        else if (other.CompareTag("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().SetTarget(Mytransform);
            return;
        }
        else if (other.CompareTag("Dron"))
        {
            Gun.SetDangerPoin(other.transform);
            return;
        }
    }
    private void UseLootPoint()
    {
        if (currentLootPoint != null)
        {
            if (currentLootPoint.useble && PlayerStateController.statusPack.currentMoneyCount >= currentLootPoint.cost)
            {
                if (currentLootPoint.coinsType)
                {
                    PlayerStateController.playerUIController.Spend(currentLootPoint.cost);
                }
                if (StatusPack.currentMoneyCount >= currentLootPoint.cost)
                    ActionTipMessageActivated?.Invoke(currentLootPoint.tipText, true);
                else
                    ActionTipMessageActivated?.Invoke("Не хватает монет для покупки", false);
                currentLootPoint.SetPlayer(PlayerStateController);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LootPoint") && currentLootPoint == other.GetComponent<LootPointScript>())
        {
            currentLootPoint = null;
            PlayerStateController.playerUIController.ClearTip();
            return;
        }
        else if (other.CompareTag("MoveTransform") && other == currentMoveTransformCol)
        {
            currentMoveTransformCol = null;
            transform.parent = null;
            return;
        }
        else if (other.CompareTag("EnemyView"))
        {
            other.GetComponent<ITargetTracker>().ClearTarget(transform);
            return;
        }
        else if (other.CompareTag("Dron"))
        {
            Gun.ClearDangerPoint();
            return;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Manip"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude * rb.mass > 100 - rb.mass)
            {
                GetDamage(Mathf.RoundToInt(rb.velocity.magnitude));
            }
        }
        else if (collision.collider.CompareTag("Damager"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude > 5)
            {
                GetDamage(25);
            }
        }
    }
}
