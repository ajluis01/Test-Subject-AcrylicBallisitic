using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] string musicName;

    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        audioPlayer.PlayMusic(musicName, 0.5f);
    }
}