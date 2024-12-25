using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class PlayRandomSoundOnStart : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] float volume = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        int index = Random.Range(0, audioClips.Length-1);
        _audioSource.PlayOneShot(audioClips[index], volume);
    }

   
}
