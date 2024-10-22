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
        // Where x: Horizontal (Rotation) & y: Vertical (Forward/Backward)
        this.movementVector = movementVector;

        if (movementVector.y != 0)
        {
            // Accelerate forward or backward depending on input
            speed_current = Mathf.MoveTowards(speed_current, movementVector.y * speed_max, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate smoothly when no input is given
            speed_current = Mathf.MoveTowards(speed_current, 0, deceleration * Time.deltaTime);
        }
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
        Vector2 moveDirection = transform.up * speed_current * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotation
        float rotationAmount = movementVector.x * speed_rotation * Time.deltaTime;
        rb.MoveRotation(rb.rotation - rotationAmount);
    }
    #endregion
}
