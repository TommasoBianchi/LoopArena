using System.Linq;
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

    public int maxSFXPerType = 10;
    public int SFXCapacityRechargeSpeed = 10;
    private static Dictionary<ClipType, float> activeSFXPerType = new Dictionary<ClipType, float>();

    void Start()
    {
        if (Instance != null)
        {
            throw new System.Exception("Impossible to have more than one AudioManager");
        }

        Instance = this;

        buildClipsDict();
    }

    private void Update()
    {
        ClipType[] types = activeSFXPerType.Keys.ToArray();
        foreach (var type in types)
        {
            activeSFXPerType[type] -= SFXCapacityRechargeSpeed * Time.deltaTime;
        }
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
        if (!activeSFXPerType.ContainsKey(type))
        {
            activeSFXPerType[type] = 0;
        }

        if (activeSFXPerType[type] > Instance.maxSFXPerType)
        {
            return;
        }

        List<AudioClip> allAudioClips = Instance.clipsDict[type];
        AudioClip audioClip = allAudioClips[Random.Range(0, allAudioClips.Count)];

        AudioSource.PlayClipAtPoint(audioClip, Instance.transform.position);

        // NOTE: we count SFXs by accumulating total duration and discounting every Update (this way we do not need to know when they precisely finish)
        activeSFXPerType[type] += audioClip.length;
    }

    [System.Serializable]
    public struct ClipTuple
    {
        public ClipType type;
        public List<AudioClip> clips;
    }
}
