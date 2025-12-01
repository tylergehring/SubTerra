using UnityEngine;

public class Hazard : MonoBehaviour
{
   // [SerializeField] private float damage = 20f; // Damage dealt to player
    [SerializeField] private float activationRange = 1f; // Range to trigger hazard
    [SerializeField] private float damageInterval = 1f; // Seconds between damage ticks
    [SerializeField] private float followRange = 5f;
    [SerializeField] private float moveSpeed = 5f; // How fast the hazard moves 
    [SerializeField] private float moveDuration = 2f; // How long the hazard gonna follow the player
    [SerializeField] private Vector3 gravity = new Vector3(0f, -9.8f, 0f);   // gravity
    [SerializeField] private Vector3 spawnMin; // minimum corner of the spawn Area
    [SerializeField] private Vector3 spawnMax;  // Maximum corner of the spawn area
    [SerializeField] private float respawnDelay = 5f; // Time before a new rock spawns
    public void SetActivationRange(float r) => activationRange = r;
    public void SetFollowRange(float r) => followRange = r;

    private float respawnTimer = 0f;
    private Vector3 velocity;
    private bool isMoving = false; // Flag if the hazard is currently moving 
    private float moveStartTime; // Record when the movement started 
    private PlayerController player;
    private float lastDamageTime;
    private DamageSuper damageObject = new Damage(); // static type - DamageSuper | --- Damage(), dynamic type  | compiler only allowes me to call the methods that exist inside the DamageSuper class.
    private Rigidbody2D rb; // to stop rock from passing through walls
    void Start()
    {  
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();   // Take the reference of the playercontroller script into player 
        if (player == null) Debug.LogError("Player not found! Ensure Player has 'Player' tag and PlayerController script.");
        lastDamageTime = Time.time;
        rb = GetComponent<Rigidbody2D>(); // for physics movement 
    }


    void Update()
    {
        if (player == null) return; // If player is null, the function exits immediately.

        // Check if the player is witin range to follow by the game hazard
        if (Vector3.Distance(transform.position, player.transform.position) < followRange && !isMoving)
        {
            isMoving = true;
            moveStartTime = Time.time;
            //Initialize velocity with a slight random horizontal component
            velocity = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * moveSpeed;
        }

        // Apply Gravity
        rb.linearVelocity += new Vector2(0, gravity.y) * Time.deltaTime; // gravity collides with walls.


        // Follow the player if it is in follow Range
        if (isMoving)
        {
            // Direction toward player with slight random drift
            Vector3 direction = (player.transform.position - transform.position).normalized; // Detect the direction of the player 
                                                                                             //Adds small randomness to horizontal movement
            direction.x += Random.Range(-0.2f, 0.2f);
            direction.z += Random.Range(-0.2f, 0.2f);
            direction.Normalize(); // fixes the vector length after adding random drift

            // Update velocity (horizontal movement)
            velocity.x = direction.x * moveSpeed;
            velocity.z = direction.z * moveSpeed;

            // move the hazard 
            rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y); //the rock collides with walls.



            // Apply rotation for natural falling effect
            transform.Rotate(new Vector3(90f, 0f, 45f) * Time.deltaTime);

            // Stop moving after moveDuration
            if (Time.time - moveStartTime >= moveDuration)
            {
                isMoving = false;
            }
        }

        // deal with the respawn time
        respawnTimer += Time.deltaTime;

        // Respawn logic
        if (respawnTimer > respawnDelay)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer >= respawnDelay)
            {
                Spawn(GetRandomSpawnPosition()); // spawn a new rock
                respawnTimer = 0f; // reset timer
            }
            respawnTimer = 0f;
        }

        // Check if player is within range to give damage 
        if (Vector3.Distance(transform.position, player.transform.position) < activationRange)  // vector3.Distance(a, b)  is |a-b| where a is hazard position and b is player position
        {
            if (Time.time - lastDamageTime >= damageInterval) // current game time in seconds - when the hazard last dealt damage.
            {
                player.ChangeHealth((int)damageObject.getDamage()); // Deal damage to player     // (int)damageObject.getDamage()
                lastDamageTime = Time.time;  // Record the last time when the player got damaged.
            }
        }

    }
    
    //  a function to return a random position within the bounds:
    Vector3 GetRandomSpawnPosition()
    {
        float x = player.transform.position.x + Random.Range(spawnMin.x, spawnMax.x);
        float y = player.transform.position.y + Random.Range(spawnMin.y, spawnMax.y);
        return new Vector3(x, y, 0);

    }

    public void Spawn(Vector3 position)
    {
        transform.position = position;
        isMoving = false; // Reset movement state
        Debug.Log($"Rock spawned at {position}");
   
      // World.Instance.GetTerrainHandler().DestroyInRadius(transform.position, 3);   // This prevents rock from spawning inside terrain.

    }


    // Visualize activation range in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }

    public void setDamage(float damage1)
    {
        //damage = damage1;
      damageObject.setDamage(damage1);
    }
}

public class DamageSuper 
{
    protected float damage = 5f;
    
    public void setDamage(float damage1)
    {
        damage = damage1;
    }
     public virtual float getDamage()   //Dynamic // runs at a runtimr 
    // public float getDamage()     // Static
    {
        Debug.Log($"DamageSuper: 10000f");

        return 10000f;
    }

}


public class Damage : DamageSuper
{
     public override float getDamage()  // Dynamic
   // public float getDamage()         // Static
    {
        Debug.Log($"Damage : {damage}");
   
        return damage;
    }
}
