using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform projectileSpawnPoint;
    public float speed;
    private Rigidbody2D myBody;

    public List<PlayerClone.ReplayStep> currentTrajectory { get; private set; }

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        currentTrajectory = new List<PlayerClone.ReplayStep>();
    }

    public void ResetCurrentTrajectory()
    {
        currentTrajectory = new List<PlayerClone.ReplayStep>();
    }

    void Update()
    {
        // Save current step in trajectory
        currentTrajectory.Add(new PlayerClone.ReplayStep(
            transform.position,
            transform.rotation,
            Input.GetMouseButtonDown(0)
        ));

        // Move
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        myBody.velocity = moveDirection * speed;

        // Look towards the mouse cursor
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Euler(0, 0, -Vector3.SignedAngle(lookDirection, Vector3.right, Vector3.forward));
        }

        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(PrefabsManager.GetPrefab(PrefabsManager.PrefabType.Projectile), projectileSpawnPoint.position, transform.rotation);
        }
    }
}
