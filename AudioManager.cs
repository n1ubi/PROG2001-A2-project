using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip hitSound; //audio file 1
    public AudioClip bgSound;// file2
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayBGSound()
    {
        audioSource.PlayOneShot(bgSound);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
