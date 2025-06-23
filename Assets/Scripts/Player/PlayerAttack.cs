using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAttack : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Transform[] attackPositions;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip meleeAttackSFX;
    [SerializeField] private AudioClip magicAttackSFX;

    [Header("Melee Config")]
    [SerializeField] private ParticleSystem slashFX;
    [SerializeField] private float minDistanceMeleeAttack;

    public Weapon CurrentWeapon { get; private set; }

    private PlayerActions actions;
    private PlayerAnimations playerAnimations;
    private PlayerMovement playerMovement;
    private PlayerMana playerMana;
    private EnemyBrain enemyTarget;
    private Coroutine attackCoroutine;

    private Transform currentAttackPosition;
    private float currentAttackRotation;
    private bool isFrozen = false;

    private void Awake()
    {
        actions = new PlayerActions();
        playerMana = GetComponent<PlayerMana>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimations>();
    }

    private void Start()
    {
        WeaponManager.Instance.EquipWeapon(initialWeapon);
        actions.Attack.ClickAttack.performed += ctx => Attack();
    }

    private void Update()
    {
        if (!isFrozen)
        {
            GetFirePosition();
        }
    }

    private void Attack()
    {
        if (isFrozen || enemyTarget == null) return;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(IEAttack());
    }

    private IEnumerator IEAttack()
    {
        if (currentAttackPosition == null) yield break;

        if (CurrentWeapon.WeaponType == WeaponType.Magic)
        {
            if (playerMana.CurrentMana < CurrentWeapon.RequiredMana) yield break;
            MagicAttack();
        }
        else
        {
            MeleeAttack();
        }

        playerAnimations.SetAttackAnimation(true);
        yield return new WaitForSeconds(0.5f);
        playerAnimations.SetAttackAnimation(false);
    }

    private void MagicAttack()
    {
        Quaternion rotation = Quaternion.Euler(0f, 0f, currentAttackRotation);
        Projectile projectile = Instantiate(CurrentWeapon.ProjectilePrefab, currentAttackPosition.position, rotation);
        projectile.Direction = Vector3.up;
        projectile.Damage = GetAttackDamage();

        playerMana.UseMana(CurrentWeapon.RequiredMana);
        BackgroundSoundManager.Instance.PlayShootSFX();
    }

    private void MeleeAttack()
    {
        slashFX.transform.position = currentAttackPosition.position;
        slashFX.Play();
        BackgroundSoundManager.Instance.PlayShootSFX();

        float distance = Vector3.Distance(enemyTarget.transform.position, transform.position);
        if (distance <= minDistanceMeleeAttack)
        {
            enemyTarget.GetComponent<IDamageable>().TakeDamage(GetAttackDamage());
            BackgroundSoundManager.Instance.PlayHitSFX();
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        CurrentWeapon = newWeapon;
        stats.TotalDamage = stats.BaseDamage + CurrentWeapon.Damage;
    }

    private float GetAttackDamage()
    {
        float damage = stats.BaseDamage + CurrentWeapon.Damage;
        if (Random.Range(0f, 100f) <= stats.CriticalChance)
        {
            damage += damage * (stats.CriticalDamage / 100f);
        }

        return damage;
    }

    private void GetFirePosition()
    {
        Vector2 moveDir = playerMovement.MoveDirection;

        if (moveDir.x > 0f)
        {
            currentAttackPosition = attackPositions[1];
            currentAttackRotation = -90f;
        }
        else if (moveDir.x < 0f)
        {
            currentAttackPosition = attackPositions[3];
            currentAttackRotation = -270f;
        }

        if (moveDir.y > 0f)
        {
            currentAttackPosition = attackPositions[0];
            currentAttackRotation = 0f;
        }
        else if (moveDir.y < 0f)
        {
            currentAttackPosition = attackPositions[2];
            currentAttackRotation = -180f;
        }
    }

    private void EnemySelectedCallback(EnemyBrain enemy)
    {
        enemyTarget = enemy;
    }

    private void NoEnemySelectionCallback()
    {
        enemyTarget = null;
    }

    public void Freeze() => isFrozen = true;
    public void Unfreeze() => isFrozen = false;

    private void OnEnable()
    {
        actions.Enable();
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoEnemySelectionCallback;
        EnemyHealth.OnEnemyDeadEvent += NoEnemySelectionCallback;
    }

    private void OnDisable()
    {
        actions.Disable();
        SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent -= NoEnemySelectionCallback;
        EnemyHealth.OnEnemyDeadEvent -= NoEnemySelectionCallback;
    }
}
