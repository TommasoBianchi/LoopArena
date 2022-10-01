using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum ClipType
    {
        Shoot,
        Hit
    }

    private static AudioManager Instance;

    [SerializeReference]
    public List<AudioClip> clips;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one AudioManager");
        }

        Instance = this;
    }

    public static void Play(ClipType type)
    {
        switch (type)
        {
            case ClipType.Shoot:
                // AudioSource.PlayClipAtPoint(clips[0], new Vector3(0, 0, 0));
                return;
            case ClipType.Hit:
                return;
            default:
                return;
        }
    }
}
