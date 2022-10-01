using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isFirstCheckpoint;
    public int maxClonesSupported;

    private static Stack<Checkpoint> checkpoints = new Stack<Checkpoint>();
    public static Checkpoint Current { get { return checkpoints.Peek(); } }

    private void Start()
    {
        if (isFirstCheckpoint)
        {
            checkpoints.Push(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (!isFirstCheckpoint && player != null && Current != this)
        {
            // The player has hit a new checkpoint, add it to the stack and snapshot the world
            checkpoints.Push(this);
            TimeManager.Instance.CreateSnapshot();
        }
    }
}
