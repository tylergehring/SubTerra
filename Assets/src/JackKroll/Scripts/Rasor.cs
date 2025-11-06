using UnityEngine;
using System.Collections;

public class Rasor : UtilityTool
{
    private AudioSource audioSource;
    private Transform player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (player != null && gameObject.activeSelf)
        {
            // Keep Rasor positioned relative to player
            Vector3 pos = transform.position;
            pos.x = player.position.x + 0.6f;
            pos.y = player.position.y + 0.1f;
            pos.z = -1f; // fixed Z position
            transform.position = pos;

            // Trigger the tool when left mouse button is pressed
            if ((Input.GetMouseButton(0) && !Input.GetKeyDown(KeyCode.F)) || ((!Input.GetMouseButton(0) && Input.GetKeyDown(KeyCode.F)))) // Left mouse button held down
            {
                UseTool();
            }
        }
    }

    // Dynamic binding: called when the tool is used
    public override void UseTool(GameObject target = null)
    {
        StartCoroutine(PlaySoundForTwoSeconds());
    }

    private IEnumerator PlaySoundForTwoSeconds()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            yield return new WaitForSeconds(0.8f);
            audioSource.Stop();
        }
    }
}
//no dynamic bynding 
/*
using UnityEngine;
using System.Collections;
public class Rasor : UtilityTool
{
    private AudioSource audioSource;

    public Transform player;

    private float rotationSpeed = 180f; // degrees per second


    private void Start()
    {


        audioSource = GetComponent<AudioSource>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }


    void Update()
    {

        //cnages position 
        if (gameObject.activeSelf) // checks if the prefab (this GameObject) is active
        {
            transform.position = player.position + player.forward * 1f;



            // Keep Z position fixed at 1
            var pos = transform.position;
            pos.x = player.position.x + 0.6f;
            pos.y = player.position.y + 0.1f;
            pos.z = -1f;
            transform.position = pos;




            if (Input.GetMouseButton(0)) // Left mouse button held down
            {
                // Rotate around the Y axis (you can change to X/Z as needed)
                //  transform.Rotate(0f, rotationSpeed * Time.deltaTime, 90f);
                transform.localPosition = player.forward * Mathf.PingPong(Time.time * 4f, 0.3f);


                StartCoroutine(PlayForTwoSeconds());





                
                //if (!audioSource.isPlaying)
                  //  {
                    //    audioSource.Play();

 //                        yield return new WaitForSeconds(2f);
   //                 }


                }
     //           else
       //         {
         //           if (audioSource.isPlaying)
           //         {
             //           audioSource.Stop();
               //     }
                
           // }
            }


        
       // if (Input.GetMouseButtonDown(0))
       // {
        //    InflictDammage();
       // }
    }
    IEnumerator PlayForTwoSeconds()
    {
        audioSource.Play();
        yield return new WaitForSeconds(1f); 
        audioSource.Stop();
    }

    //  private void InflictDammage()
    // {

    //  }
}
*/