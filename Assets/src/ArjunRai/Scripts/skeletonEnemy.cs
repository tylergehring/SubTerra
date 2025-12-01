using System.Collections;
using UnityEngine;

public class skeletonEnemy : MonoBehaviour
{
    private PlayerController player; // Reference for the player 
    private Transform playerTransform; // Reference to the player's transform 
    private bool playerInRange = false; // True when the player enters the detection range 
    private bool canThrow = true;  // Prevent bone spamming - cooldown
    private Animator animator;  // Animation controller
    private Rigidbody2D rb;
    private bool isDead = false;
    [SerializeField] private float moveSpeed = 1.0f;   // Skeleton move speed 
    [SerializeField] private float stopRange = 2.0f;   // Range before the skeleton stops 
    [SerializeField] private float throwRange = 4f;   // Range for the skeleton to throw the bone 
    [SerializeField] private float throwCooldown = 1.2f;   // Cooldown time after throwing a bone
    [SerializeField] private GameObject Bone;  // Bone prefab to spawn 
    [SerializeField] private GameObject LeftHand; // Bone spawning point 
    [SerializeField] private int maxHealth = 50;

    private int currentHealth; // Tracks current health

    private void Start()
    {
        // Check Singleton exists
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController Singleton is missing!");
            return;
        }

        // Get the script instance
        player = PlayerController.Instance;

        // Get the actual Player GameObject from the Singleton
        GameObject playerObj = PlayerController.Instance.GetObject();

        if (playerObj == null)
        {
            Debug.LogError("Player Object is missing from Singleton!");
            return;
        }

        // Store the player's Transform safely
        playerTransform = playerObj.transform;

        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (isDead) return;

        // Prevent null crashes
        if (playerTransform == null) return;

        if (!playerInRange) return;     // If player is not in range, do nothing

        float distance = Vector2.Distance(transform.position, playerTransform.position);  // Distance to the player

        // Flip left/right
        if (playerTransform.position.x > transform.position.x)
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);

        // Throw bone if close enough
        if (distance <= throwRange && canThrow)
        {
            StartCoroutine(ThrowBone());
        }

        // Move if too far
        if (distance > stopRange)
        {
            animator.SetFloat("Speed", 1f); // walking animation
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            
        }
        else
        {
            animator.SetFloat("Speed", 0f); // idle animation
        }
    }

    // Coroutine for bone throwing
    private IEnumerator ThrowBone()
    {
        canThrow = false;

        animator.SetTrigger("Attack");  // Play attack animation
        animator.SetFloat("Speed", 0f); // Stop moving during attack 

        yield return new WaitForSeconds(0.25f); // Timing for attack animation

        GameObject attack = Instantiate(Bone, LeftHand.transform.position, Quaternion.identity);

        Vector2 direction = (playerTransform.position - LeftHand.transform.position).normalized;

        attack.GetComponent<Rigidbody2D>().AddForce(direction * 800f);

        Destroy(attack, 3f); // Remove the bone after 3 seconds

        SoundEvents.EnemyThrow();   // Mikayla  -   Trigger Sound Event

        yield return new WaitForSeconds(throwCooldown);   // Wait for cooldown

        canThrow = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy took damage, remaining health: " + currentHealth);

        SoundEvents.EnemyDamage();  // Mikayla - Notify SoundManager

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;  // Prevent double death
        isDead = true;

        animator.SetTrigger("Die"); // animation for enemy dying 

        // stop movement 
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        // Disable colliders so no more interactions
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // Stop attacking
        canThrow = false;
        StopAllCoroutines();

        // Destroy after animation
        Destroy(gameObject, 1.2f);

    }
}
