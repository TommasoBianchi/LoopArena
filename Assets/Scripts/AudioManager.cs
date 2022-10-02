using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum ClipType
    {
        Shoot,
        Hit,
        MonsterNoise,
        MonsterDeath,
        Reset,
        ActivateCheckpoint,
        Walk
    }

    private static AudioManager Instance;

    public List<ClipTuple> clips;
    private Dictionary<ClipType, List<AudioClip>> clipsDict;

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one AudioManager");
        }

        Instance = this;

        buildClipsDict();
    }

    private void buildClipsDict()
    {
        clipsDict = new Dictionary<ClipType, List<AudioClip>>();

        foreach (var item in clips)
        {
            if (clipsDict.ContainsKey(item.type))
            {
                throw new System.Exception("Cannot contain more than one clip per type");
            }

            clipsDict[item.type] = item.clips;
        }
    }

    public static void Play(ClipType type)
    {
        switch (type)
        {
            case ClipType.Shoot:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.Shoot][0], Instance.transform.position);
                return;
            case ClipType.Hit:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.Hit][0], Instance.transform.position);
                return;
            case ClipType.MonsterNoise:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.MonsterNoise][0], Instance.transform.position);
                return;
            case ClipType.MonsterDeath:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.MonsterDeath][0], Instance.transform.position);
                return;
            case ClipType.Reset:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.Reset][0], Instance.transform.position);
                return;
            case ClipType.ActivateCheckpoint:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.ActivateCheckpoint][0], Instance.transform.position);
                return;
            case ClipType.Walk:
                AudioSource.PlayClipAtPoint(Instance.clipsDict[AudioManager.ClipType.Walk][0], Instance.transform.position);
                return;
            default:
                return;
        }
    }

    [System.Serializable]
    public struct ClipTuple
    {
        public ClipType type;
        public List<AudioClip> clips;
    }
}
