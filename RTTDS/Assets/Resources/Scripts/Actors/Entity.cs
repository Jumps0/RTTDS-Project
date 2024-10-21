using UnityEditor.Rendering;
using UnityEngine;

/// <summary>
/// A generic class used to represent any actionable entity and store its key information.
/// </summary>
public class Entity : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;

    [Header("Values")]
    [Header("- Movement")]
    private Vector2 movementVector;
    public float speed_max = 50f;
    public float speed_rotation = 150f;
    public float speed_rotationTurret = 150f;
    public float acceleration = 70f;
    public float deceleration = 50f;
    public float speed_current = 0;
    public float currentForwardDirection = 1;
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

    public void Move(Vector2 movementVector)
    {
        this.movementVector = movementVector;

        CalculateSpeed(movementVector);

        if(movementVector.y > 0)
        {
            currentForwardDirection = 1;
        }
        else if (movementVector.y < 0)
        {
            currentForwardDirection = 0;
        }

    }

    private void CalculateSpeed(Vector2 movementVector)
    {
        if(Mathf.Abs(movementVector.y) > 0)
        {
            speed_current += acceleration * Time.deltaTime;
        }
        else
        {
            speed_current -= deceleration * Time.deltaTime;
        }
        speed_current = Mathf.Clamp(speed_current, 0, speed_max);
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
        rb.linearVelocity = (Vector2)transform.up * speed_current * currentForwardDirection * Time.fixedDeltaTime;
        // Rotation
        rb.MoveRotation(transform.rotation * Quaternion.Euler(0, 0, -movementVector.x * speed_rotation * Time.fixedDeltaTime));
    }
    #endregion
}
