using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject tracking;
    [SerializeField] private GameObject player;
    [SerializeField] private float sound_bound;
    [SerializeField] private AudioClip clip;
    private AudioSource sound;

    private void Awake(){
        if (player == null){
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (clip != null){
            sound = gameObject.AddComponent<AudioSource>();
            sound.clip = clip;
            sound.loop = true;
            sound.Play();
        }
    }

    private void Update(){
        UpdateVol();
    }

    private void UpdateVol(){
        if (clip == null || player == null || tracking == null || sound == null){
            return;
        }

        if (sound_bound <= 0){
            return;
        }

        float distance = Vector3.Distance(tracking.transform.position, player.transform.position);

        if (distance >= sound_bound){
            sound.volume = 0.01f;
        }
        else {
            sound.volume = 1.0f - (distance/sound_bound);
        }
    }
}
