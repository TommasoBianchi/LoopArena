using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isFirstCheckpoint;
    public int maxClonesSupported;
    public int durability;
    public float durabilityReloadPerSecond;

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

                // TEMP (useful for visualization)
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void DecreaseDurability()
    {
        currentDurability -= 1;

        if (!isFirstCheckpoint && isActive && currentDurability <= 0)
        {
            // Disable the checkpoint (TODO: edit graphics accordingly) and redirect to the previous one
            isActive = false;
            checkpoints.Pop();
            TimeManager.Instance.RollbackSnapshot();

            // TEMP (useful for visualization)
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (!isFirstCheckpoint && isActive && player != null && Current != this)
        {
            // The player has hit a new checkpoint, add it to the stack and snapshot the world
            AudioManager.Play(AudioManager.ClipType.ActivateCheckpoint);
            checkpoints.Push(this);
            TimeManager.Instance.CreateSnapshot();

            // Reset the current player recorded trajectory
            player.ResetCurrentTrajectory();
        }
    }
}
