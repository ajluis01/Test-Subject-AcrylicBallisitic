using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class AutoAudioPlayer : MonoBehaviour
{
    [SerializeField] string soundName;

    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        audioPlayer.PlaySound(soundName, 1f, true);
    }
}