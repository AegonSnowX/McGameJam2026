using UnityEngine;

/// <summary>
/// Plays a single clip in a loop for the whole scene (e.g. menu music or main-scene ambience).
/// Add to a GameObject in the scene and assign the clip; it starts on Start and stops when the scene unloads.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundAudio : MonoBehaviour
{
    [Header("Clip")]
    [SerializeField] private AudioClip clip;

    [Header("Optional")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private AudioSource _source;

    void Start()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null) return;

        AudioClip toPlay = clip != null ? clip : _source.clip;
        if (toPlay == null)
        {
            Debug.LogWarning("[BackgroundAudio] " + gameObject.name + ": No clip assigned.", this);
            return;
        }

        _source.clip = toPlay;
        _source.volume = volume;
        _source.loop = true;
        _source.playOnAwake = false;
        _source.Play();
    }
}
