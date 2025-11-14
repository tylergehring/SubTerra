using System.Collections;
using UnityEngine;

public class skeletonEnemy : MonoBehaviour
{
    private PlayerController player; // Reference for the player 
    private Transform playerTransform; // Reference to the player transform 
    private bool playerInRange = false; // True when the player enter the detection range 
    private bool canThrow = true;  // prevent bone spamming- cooldown
    private Animator animator;  // Animation controoller

    [SerializeField] private float moveSpeed = 1.0f;   // Skeleton move speed 
    [SerializeField] private float stopRange = 2.0f;   // Range before the skeleton stops 
    [SerializeField] private float throwRange = 4f;   // Range for the skeleton to throw the bone 
    [SerializeField] private float throwCooldown = 1.2f;   // cool dowm time after each time skeleton throw the bone.
    [SerializeField] private GameObject Bone;  // Bone prefab to spawn 
    [SerializeField] private GameObject LeftHand; //Bone spawning point 
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;                  // Tracks current health
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>(); // find the user using the "player tag"

        if (player == null)
        {
            Debug.LogError("Player not found! Ensure tag is Player.");   
            return;
        }

        playerTransform = player.transform;  // Store the player's Transform so we can easily check its position later
        animator = GetComponent<Animator>(); // Get animator 
        currentHealth = maxHealth; // Set current health at start
    }

    private void Update()
    {
        if (!playerInRange) return;     // If player is not in range do nothing return

        float distance = Vector2.Distance(transform.position, playerTransform.position);  // Distance between and the skeleton 

        // Flip left/right
        if (playerTransform.position.x > transform.position.x)
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        else
            transform.localScale = new Vector3(-0.5f, 0.5f, 1);

        // Throw bone if close enough
        if (distance <= throwRange && canThrow)
        {
            StartCoroutine(ThrowBone());
            // return; // stop movement during throw
        }

        // Move if too far
        if (distance > stopRange)
        {
            animator.SetFloat("Speed", 1f); // walking animation

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );
        }
        else
        {
            animator.SetFloat("Speed", 0f); // idle animation
        }
    }

    // coroutine 
    private IEnumerator ThrowBone()
    {
        canThrow = false;

        animator.SetTrigger("Attack");  // Play throwing animation

        animator.SetFloat("Speed", 0f); //  Sets Speed to 0 so the skeleton switches to an idle/attack pose 

        yield return new WaitForSeconds(0.25f); // Wait for animation timing

        GameObject attack = Instantiate(Bone, LeftHand.transform.position, Quaternion.identity); // spawns a new Bone object at the skeleton’s left hand.

        Vector2 direction = (playerTransform.position - LeftHand.transform.position).normalized;  // Calculate the direction toward the player

        attack.GetComponent<Rigidbody2D>().AddForce(direction * 800f);  //Add force so the bone flies

        Destroy(attack, 3f);   // Destroy bone after 3 seconds

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


    // Call this function when the enemy takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health

        Debug.Log("Enemy took damage, remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die(); // If health is zero or less, kill the enemy
        }
    }

    private void Die()
    {


    }



}
