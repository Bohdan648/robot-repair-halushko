using UnityEngine;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(AudioSource))]
public class GameFeedback : MonoBehaviour
{
    public static GameFeedback instance { get; private set; }

    public AudioClip damageClip;
    public AudioClip healClip;
    public AudioClip repairClip;
    [Range(0, 1)] public float volume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0;
    }

    public void PlayDamage() => Play(damageClip);
    public void PlayHeal() => Play(healClip);
    public void PlayRepair() => Play(repairClip);

    private void Play(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
