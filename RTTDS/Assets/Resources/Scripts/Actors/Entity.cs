using UnityEngine;

/// <summary>
/// A generic class used to represent any actionable entity and store its key information.
/// </summary>
public class Entity : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Values")]
    private Vector2 movementVector;
    public float speed_max = 50f;
    public float speed_rotation = 150f;
    public float speed_rotationTurret = 150f;
    //
    public string uniqueName;
    // ...

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void AddToGameManager()
    {
        GameManager.inst.Entities.Add(this);
    }

    #region Input
    public void HandleShoot()
    {
        // TODO
        Debug.Log($"{this.gameObject.name} is shooting.");
    }

    public void HandleMoveBody(Vector2 movementVector)
    {
        this.movementVector = movementVector;
    }

    public void HandleTurretMovement(Vector2 pointerPosition)
    {
        Transform turret = this.GetComponent<Actor>().sprite_mount.transform;

        var turretDirection = (Vector3)pointerPosition - turret.position;
        var desiredAngle = Mathf.Atan2(turretDirection.y, turretDirection.x) * Mathf.Rad2Deg;
        var rotationStep = speed_rotationTurret * Time.deltaTime;

        turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.Euler(0, 0, desiredAngle - 90), rotationStep);
    }

    public void FixedUpdate()
    {
        // Velocity
        rb.linearVelocity = (Vector2)transform.up * movementVector.y * speed_max * Time.fixedDeltaTime;
        // Rotation
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0, 0, -movementVector.x * speed_rotation * Time.fixedDeltaTime));
    }
    #endregion
}
