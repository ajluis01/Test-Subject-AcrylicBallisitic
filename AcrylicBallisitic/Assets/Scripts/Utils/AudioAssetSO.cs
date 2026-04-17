using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioAsset
{
    public bool enabled = true;
    public string name;
    public AudioClip clip;
    public List<AudioClip> variations;
}

[CreateAssetMenu(fileName = "NewAudioAsset", menuName = "Audio/AudioAsset")]
public class AudioAssetSO : ScriptableObject
{
    public List<AudioAsset> audioAssets;

    public AudioClip GetClipByName(string name, out bool enabled)
    {
        AudioAsset asset = audioAssets.Find(a => a.name == name);
        if (asset == null)
        {
            Debug.LogWarning($"Audio asset with name '{name}' not found.");
            enabled = false;
            return null;
        }
        else
        {
            enabled = asset.enabled;
            if (asset.variations != null && asset.variations.Count > 0)
            {
                List<AudioClip> fullList = new List<AudioClip>(asset.variations);
                fullList.Add(asset.clip);
                int randomIndex = UnityEngine.Random.Range(0, fullList.Count);
                return fullList[randomIndex];
            }
        }
        return asset != null ? asset.clip : null;
    }
}