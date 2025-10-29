using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClip player_jump_clip;
    [SerializeField] private AudioClip background_clip;
    [SerializeField] private float background_volume;

    private AudioSource backgroundSource;
    private AudioSource jumpSource;

    private void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        backgroundSource = gameObject.AddComponent<AudioSource>();
        jumpSource = gameObject.AddComponent<AudioSource>();
    }
    private void Start() {
        BackgroundMusic();
    }
    public void BackgroundMusic(){
        if (background_clip == null){
            return;
        }
        AudioSource sound = backgroundSource;
        sound.clip = background_clip;
        sound.loop = true;
        sound.volume = Mathf.Clamp(background_volume, 0f, 0.5f);
        sound.Play();
    }
    public void JumpSound(){
        if (player_jump_clip == null){
            return;
        }
        AudioSource sound = jumpSource;
        sound.clip = player_jump_clip;
        sound.volume = Mathf.Clamp(background_volume, 0f, 1f);
        sound.Play();
    }
    //test methods
    public void SetJumpClip(AudioClip clip)
    {
        player_jump_clip = clip;
    } 
    public void SetBackgroundClip(AudioClip clip) {
        background_clip = clip;
    }
    public void SetBackgroundVolume(float volume) {
        background_volume = volume;
    } 

    //Access Methods
   
}
