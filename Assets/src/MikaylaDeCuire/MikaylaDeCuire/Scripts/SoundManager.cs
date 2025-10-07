using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip player_jump_clip;
    [SerializeField] private AudioClip background_clip;
    [SerializeField] private float background_volume;

    private void Start(){
        BackgroundMusic();
    }

    public void BackgroundMusic(){
        if (background_clip == null){
            return;
        }
        AudioSource sound = gameObject.AddComponent<AudioSource>();
        sound.clip = background_clip;
        sound.loop = true;
        sound.volume = background_volume >= 0 ? background_volume : 0;
        sound.Play();
    }

    public void JumpSound(){
        if (player_jump_clip == null){
            return;
        }
        AudioSource sound = gameObject.AddComponent<AudioSource>();
        sound.clip = player_jump_clip;
        sound.Play();
    }
}
