using UnityEngine;
using System.Resources;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// A broad class for an individual player to be able to control one or more units.
/// </summary>
public class UnitController : MonoBehaviour
{
    [Header("Units")]
    [Tooltip("The unit that this controller is DIRECTLY controlling.")]
    public Actor directControl;
    [Tooltip("A list of units IN-DIRECTLY controlled by this controller. Changes rapidly.")]
    public List<Actor> indirectControl = new List<Actor>();

    [Header("Input Events")]
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnMoveTurret = new UnityEvent<Vector2>();

    public Camera mainCamera;

    private void Awake()
    {
        // Change this later if needed
        mainCamera = Camera.main;
    }

    public void Update()
    {
        if (directControl != null)
        {
            CheckDirectInput();
        }
    }

    #region Utilities
    public void SetDirectControl(Actor d)
    {
        d.controller = this;
        d = directControl;

        if (indirectControl.Contains(d))
        {
            indirectControl.Remove(d);
        }
    }

    public void DropDirectControl(Actor d)
    {
        d.controller = null;
        d = null;
    }

    #endregion

    #region DIRECT - Input
    private void CheckDirectInput()
    {
        Actor p = directControl;
        if (p != null) // Just to be safe
        {
            InputBody(p);
            InputTurret(p);
            InputFiring(p);
        }
    }

    private void InputBody(Actor p)
    {
        Vector2 movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMoveBody?.Invoke(movementVector.normalized);
    }

    private void InputTurret(Actor p)
    {
        // TODO: Check if the player should be allowed to move the turret
        OnMoveTurret?.Invoke(GetMousePosition());
    }

    private void InputFiring(Actor p)
    {
        // TODO: Check if the player actually has a weapon and all the prereqs are met for firing it
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            OnShoot?.Invoke();
        }
    }

    private Vector2 GetMousePosition() // Based on work by: Sunny Valley Studio - https://www.youtube.com/watch?v=monYp9VlBy4&list=PLcRSafycjWFfIzbU5gqa6PSIsIVe-Acw5&index=2
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane;
        Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        return mouseWorldPosition;
    }

    #endregion
}
