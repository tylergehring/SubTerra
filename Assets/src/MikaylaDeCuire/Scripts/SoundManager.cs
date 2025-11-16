using UnityEngine;
using System;

public class SoundManager : MonoBehaviour {

    /* Singleton Pattern 
    Public - Other scripts can reference it
    Static - Belongs to this class not individual objects
    Instance - One shared across the whole program
    set is private so that only the SoundManager class can reassign to Instance
    get is left public by default so other scripts can read SoundManager.Instance
    */
    public static SoundManager Instance { get; private set; }

    // The Headers show up in Unity, Added for readability since I'm adding SO many sound files

    [Header("Player Clips")]
    [SerializeField] private AudioClip playerJump;
    [SerializeField] private AudioClip[] footStep;
    [SerializeField] private AudioClip enemyDamageClip;

    [Header("Tool Clips")]
    [SerializeField] private AudioClip useTool;
    [SerializeField] private AudioClip addTool;

    [Header("Background Music")]
    [SerializeField] private AudioClip background_clip;
    [SerializeField] private float background_volume = 0.3f;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    /* Runs once when script is loaded (before Start and OnEnable)
    Use for initialization that must happen before anything else
    Singleton check - Ensure only one SM exists (if another is already active --> destory yourself)
    DontDestroyOnLoad keeps the manager across scene changes (persistent audio like background music)
    */
    private void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    /* Runs whenever the gameObject is in the scene
    Observer check  -  Subscribe SoundManager to events
    SoundEvents.PlayerJump() is called --> SoundManager reacts with PlayJumpSound()
    Things here need to be done after Awake once scene is fully initialized 
    */
    private void OnEnable()
    {
        SoundEvents.OnPlayerJump += PlaySFX(playerJump);
        SoundEvents.OnToolUse += PlaySFX(useTool);
        SoundEvents.OnToolPickup += PlaySFX(addTool);
        SoundEvents.OnFootstep += PlayFootstep;
        SoundEvents.OnEnemyDamage += () => PlaySFX(enemyDamageClip);
        SoundEvents.OnEnemyThrow += () => PlaySFX(enemyThrowClip);
        SoundEvents.OnToolUse += () => PlaySFX(toolUseClip);
    }

    /* Runs when the gameObject = destroyed
    Unsubscribe to events so that Unity doesnt keep references to destroyed objects
    */
    private void OnDisable()
    {
        SoundEvents.OnPlayerJump -= PlaySFX(playerJump);
        SoundEvents.OnToolUse -= PlaySFX(useTool);
        SoundEvents.OnToolPickup -= PlaySFX(addTool);
        SoundEvents.OnFootstep -= PlayFootstep;
        SoundEvents.OnEnemyDamage -= () => PlaySFX(enemyDamageClip);
        SoundEvents.OnEnemyThrow -= () => PlaySFX(enemyThrowClip);
        SoundEvents.OnToolUse -= () => PlaySFX(toolUseClip);
    }

    // Runs once after Awake when scene is fully initialized
    private void Start() { BackgroundMusic();}

    public void BackgroundMusic(){
        if (background_clip == null || backgroundSource == null || this == null) return;

        backgroundSource.clip = background_clip;
        backgroundSource.loop = true;
        backgroundSource.volume = Mathf.Clamp(background_volume, 0f, 0.5f);
        backgroundSource.Play();
    }
    private void PlayFootstep()
    {
        if (footStep == null || footStep.Length == 0) return;

        var idx = UnityEngine.Random.Range(0, footStep.Length);
        var clip = footStep[idx];
        if (clip != null) PlaySFX(clip);

        if (clip == null) return; // avoid crashing
        PlaySFX(clip);
    }

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
}
