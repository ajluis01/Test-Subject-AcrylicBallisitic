using PrimeTween;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioAssetSO audioAssets;

    AudioSource musicAudioSource;

    float normalVolume = 1.0f;
    bool isVolumeReduced = false;

    public void PlaySound(string name, float volume = 1.0f, bool shouldLowerMusic = false)
    {
        if (audioAssets == null)
        {
            Debug.LogWarning("No audio asset");
            return;
        }
        AudioClip clip = audioAssets.GetClipByName(name, out bool enabled);
        if (clip != null && enabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
            if (shouldLowerMusic)
            {
                EventManager.Broadcast(new LowerMusicVolumeEvent(true));
            }
            Tween.Custom(0f, 1f, clip.length, (v) => {}).OnComplete(() =>
            {
                if (shouldLowerMusic)
                {
                    EventManager.Broadcast(new LowerMusicVolumeEvent(false));
                }
            });
        }
    }

    public void PlayMusic(string name, float volume = 1.0f)
    {
        if (audioAssets == null)
        {
            Debug.LogWarning("No audio asset");
            return;
        }
        AudioClip clip = audioAssets.GetClipByName(name, out bool enabled);
        if (clip != null && enabled)
        {
            musicAudioSource = GetComponent<AudioSource>();
            if (musicAudioSource == null) return;
            musicAudioSource.clip = clip;
            musicAudioSource.volume = volume;
            normalVolume = volume;
            musicAudioSource.Play();

            EventManager.AddListener<LowerMusicVolumeEvent>(OnLowerMusicVolume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicAudioSource != null)
        {
            Tween.AudioVolume(musicAudioSource, volume, 1.5f, Ease.InOutCubic);
        }
    }

    void Update()
    {
        if (!isVolumeReduced)
        {
            if (GameManager.GetManager() != null && GameManager.GetManager().IsGracePeriod())
            {
                SetMusicVolume(0.15f);
                isVolumeReduced = true;
            }
        }
        else
        {
            if (GameManager.GetManager() == null || !GameManager.GetManager().IsGracePeriod())
            {
                SetMusicVolume(normalVolume);
                isVolumeReduced = false;
            }
        }
    }

    void OnLowerMusicVolume(LowerMusicVolumeEvent e)
    {
        if (e.shouldLower)
        {
            SetMusicVolume(0.15f);
            
        }
        else
        {
            SetMusicVolume(normalVolume);
        }
    }
}