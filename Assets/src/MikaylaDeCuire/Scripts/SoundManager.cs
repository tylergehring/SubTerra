using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{

    /* Singleton Pattern 
    Public - Other scripts can reference it
    Static - Belongs to this class not individual objects
    Instance - One shared across the whole program
    set is private so that only the SoundManager class can reassign to Instance
    get is public so other scripts can read SoundManager.Instance
    */
    public static SoundManager Instance { get; private set; }

    [Header("Player Clips")]
    [SerializeField] private AudioClip playerJump;
    [SerializeField] private AudioClip[] footStep;
    [SerializeField] private AudioClip enemyDamage;
    [SerializeField] private AudioClip enemyThrow;
    [SerializeField] private AudioClip playerDeath;

    [Header("Tool Clips")]
    [SerializeField] private AudioClip useTool;
    [SerializeField] private AudioClip addTool;

    [Header("Background Music")]
    [SerializeField] private AudioClip background_clip;
    [SerializeField] private float background_volume = 0.3f;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    /* Runs once when script is loaded (before Start and OnEnable)
    Singleton - Ensure only one SM exists (if another is already active --> destory yourself)
    */
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    /* Runs whenever the gameObject is in the scene after Awake
    Observer  -  Subscribe SoundManager to events
    SoundEvents.PlayerJump() is broadcasted --> SoundManager reacts with PlayJumpSound()
    */
    private void OnEnable()
    {
        SoundEvents.OnPlayerJump += PlayJumpSound;
        SoundEvents.OnToolUse += PlayToolUseSound;
        SoundEvents.OnToolPickup += PlayToolPickupSound;
        SoundEvents.OnFootstep += PlayFootstep;
        SoundEvents.OnEnemyDamage += PlayEnemyDamageSound;
        SoundEvents.OnEnemyThrow += PlayEnemyThrowSound;
        SoundEvents.OnPlayerDeath += PlayDeathSound;
    }


    /* Runs when the gameObject = destroyed
    Unsubscribe to events
    */
    private void OnDisable()
    {
        SoundEvents.OnPlayerJump -= PlayJumpSound;
        SoundEvents.OnToolUse -= PlayToolUseSound;
        SoundEvents.OnToolPickup -= PlayToolPickupSound;
        SoundEvents.OnFootstep -= PlayFootstep;
        SoundEvents.OnEnemyDamage -= PlayEnemyDamageSound;
        SoundEvents.OnEnemyThrow -= PlayEnemyThrowSound;
        SoundEvents.OnPlayerDeath -= PlayDeathSound;
    }

    // Runs once after Awake when scene is fully initialized
    private void Start() { BackgroundMusic(); }

    public void BackgroundMusic()
    {
        if (background_clip == null || backgroundSource == null || this == null) return;

        backgroundSource.clip = background_clip;
        backgroundSource.loop = true;
        backgroundSource.volume = Mathf.Clamp(background_volume, 0f, 0.5f);
        backgroundSource.Play();
    }

    // Add an array of footstep audio clips
    private void PlayFootstep()
    {
        if (footStep == null || footStep.Length == 0) return;

        var idx = UnityEngine.Random.Range(0, footStep.Length);
        var clip = footStep[idx];

        if (clip != null) PlaySFX(clip);
    }

    private void PlayJumpSound() { PlaySFX(playerJump); }
    private void PlayToolUseSound() { PlaySFX(useTool); }
    private void PlayToolPickupSound() { PlaySFX(addTool); }
    private void PlayEnemyDamageSound() { PlaySFX(enemyDamage); }
    private void PlayEnemyThrowSound() { PlaySFX(enemyThrow); }
    private void PlayDeathSound() { PlaySFX(playerDeath); }


    /* Helper Method
    PlayOneShot() - Plays audio clip once wihtout interrupting other sounds the AudioSource is playing
    Allows multiple overlapping sounds to be triggered without cutting each other off
    Versus Play() which waits to start/stops each clip
    */
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    /* Hypothetical code from another developer's repo
    private void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            CustomAudioPool.Instance.Play(clip);
        }
    */
    
    /* Copyright would also be purchasing a sound clip "not intended for commercial use"
    In a game that I plan to sell or monetize. In that case I would be exceeding the licensing terms
    */  
}
