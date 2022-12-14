using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isFirstCheckpoint;
    public int maxClonesSupported;
    public int durability;
    public float durabilityReloadPerSecond;
    [SerializeField] private List<GameObject> glowList;

    public SpriteRenderer columnRenderer;
    public List<SpriteRenderer> glowRenderers;
    public Color enabledGlowColor;
    public Color disabledGlowColor;
    public Color currentGlowColor;

    private float currentDurability;
    private bool isActive;

    private static Stack<Checkpoint> checkpoints = new Stack<Checkpoint>();
    public static Checkpoint Current { get { return checkpoints.Peek(); } }

    private void Start()
    {
        if (isFirstCheckpoint)
        {
            checkpoints.Push(this);
        }

        currentDurability = durability;
        isActive = true;

    }

    private void Update()
    {
        if (Current != this)
        {
            currentDurability = Mathf.Clamp(currentDurability + durabilityReloadPerSecond * Time.deltaTime, 0, durability);

            if (!isActive && currentDurability >= durability)
            {
                // Enable the checkpoint
                isActive = true;

                // Restore opacity of all renderers (to signal checkpoint is enabled)
                foreach (var spriteRenderer in glowRenderers)
                {
                    Color color = enabledGlowColor;
                    spriteRenderer.color = color;
                }
            }
        }

        updateDurabilityGraphics();
    }

    public void DecreaseDurability()
    {
        currentDurability -= 1;

        if (!isFirstCheckpoint && isActive && currentDurability <= 0)
        {
            // Disable the checkpoint and redirect to the previous one
            isActive = false;
            checkpoints.Pop();
            TimeManager.Instance.RollbackSnapshot();

            // Reactivate child
            for (int i = 0; i < glowList.Count; i++)
            {
                glowList[i].gameObject.SetActive(true);
            }

            // Restore opacity of all renderers (to signal checkpoint is enabled) for new current checkpoint
            foreach (var spriteRenderer in Current.glowRenderers)
            {
                spriteRenderer.color = Current.currentGlowColor;
            }

            // Reduce opacity to signal checkpoint is disabled
            foreach (var spriteRenderer in glowRenderers)
            {
                spriteRenderer.color = disabledGlowColor;
            }

            // Spawn activation effect at location of new current checkpoint (only if new current is not the starting one)
            if (!Current.isFirstCheckpoint)
            {
                Instantiate(PrefabsManager.GetPrefab(PrefabsManager.PrefabType.CheckpointActivateEffect), Current.transform.position + new Vector3(0, -2.5f, 0), Quaternion.identity);
            }
        }
    }

    private void updateDurabilityGraphics()
    {
        float durabilityRatio = currentDurability / (float)durability;
        int durabilityQuota = Mathf.FloorToInt(durabilityRatio * glowList.Count);

        for (int i = 0; i < glowList.Count; ++i)
        {
            glowList[i].gameObject.SetActive(isFirstCheckpoint || i < durabilityQuota);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (!isFirstCheckpoint && isActive && player != null && Current != this)
        {
            // Update glow colors
            foreach (var spriteRenderer in Current.glowRenderers)
            {
                spriteRenderer.color = Current.enabledGlowColor;
            }

            foreach (var spriteRenderer in glowRenderers)
            {
                spriteRenderer.color = currentGlowColor;
            }

            // The player has hit a new checkpoint, add it to the stack and snapshot the world
            AudioManager.Play(AudioManager.ClipType.ActivateCheckpoint);
            checkpoints.Push(this);
            TimeManager.Instance.CreateSnapshot();

            // Reset the current player recorded trajectory
            player.ResetCurrentTrajectory();

            // Spawn activation effect at location of new current checkpoint
            Instantiate(PrefabsManager.GetPrefab(PrefabsManager.PrefabType.CheckpointActivateEffect), transform.position + new Vector3(0, -2.5f, 0), Quaternion.identity);
        }
    }
}
