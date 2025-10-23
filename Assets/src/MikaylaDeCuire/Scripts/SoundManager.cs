using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip player_jump_clip;
    [SerializeField] private AudioClip background_clip;
    [SerializeField] private float background_volume;

    private AudioSource backgroundSource;
    private AudioSource jumpSource;


    private void Awake()
    {
        backgroundSource = gameObject.AddComponent<AudioSource>();
        jumpSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start() {
        BackgroundMusic();
    }

    //test methods
    public void SetJumpClip(AudioClip clip) {
        player_jump_clip = clip;
    } 
    public void SetBackgroundClip(AudioClip clip) {
        background_clip = clip;
    }
    public void SetBackgroundVolume(float volume) {
        background_volume = volume;
    } 

    //Access Methods
    public void BackgroundMusic(){
        if (background_clip == null)
        {
            return;
        }
        AudioSource sound = backgroundSource;
        sound.clip = background_clip;
        sound.loop = true;
        sound.volume = Mathf.Clamp(background_volume, 0f, 1f);
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
}
