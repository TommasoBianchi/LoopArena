using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrackersManager : MonoBehaviour
{
    public List<Transform> trackers;
    [Range(0.01f, 0.2f)]
    public float padding;

    private Player player;
    private Checkpoint[] checkpoints;

    void Start()
    {
        player = FindObjectOfType<Player>();
        checkpoints = FindObjectsOfType<Checkpoint>();
    }

    void Update()
    {
        List<Checkpoint> sortedCheckpoints = checkpoints
            .Where(c => !c.isFirstCheckpoint) // Exclude starting checkpoint
            .Where(c => c != Checkpoint.Current) // Exclude current checkpoint
            .Where(c => !new Rect(Vector2.zero, Vector2.one).Contains(Camera.main.WorldToViewportPoint(c.transform.position))) // Exclude checkpoint already visible
            .OrderBy(c => (player.transform.position - c.transform.position).sqrMagnitude).ToList();

        for (int i = 0; i < Mathf.Min(trackers.Count, sortedCheckpoints.Count); ++i)
        {
            Vector2 viewportDir = (Vector2)Camera.main.WorldToViewportPoint(sortedCheckpoints[i].transform.position) - Vector2.one / 2;
            Vector2 trackerViewportPosition = (viewportDir / Mathf.Max(Mathf.Abs(viewportDir.y), Mathf.Abs(viewportDir.y)) + Vector2.one) / 2;
            trackerViewportPosition.x = Mathf.Clamp(trackerViewportPosition.x, padding / 2, 1 - padding / 2);
            trackerViewportPosition.y = Mathf.Clamp(trackerViewportPosition.y, padding / 2, 1 - padding / 2);
            trackers[i].transform.position = (Vector2)Camera.main.ViewportToWorldPoint(trackerViewportPosition);

            Vector2 direction = (sortedCheckpoints[i].transform.position - player.transform.position).normalized;
            trackers[i].transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, direction));

            trackers[i].gameObject.SetActive(true);
        }

        // If we have too many trackers, disable them
        for (int i = sortedCheckpoints.Count; i < trackers.Count; i++)
        {
            trackers[i].gameObject.SetActive(false);
        }
    }
}
