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
        SelectionCheck();
        if (directControl != null)
        {
            CheckDirectInput();
        }
    }

    #region Utilities
    public void SetDirectControl(Actor d)
    {
        d.controller = this;
        directControl = d;

        if (indirectControl.Contains(d))
        {
            indirectControl.Remove(d);
        }

        // Add listeners from Entity.cs
        OnShoot.AddListener(d.HandleShoot);
        OnMoveBody.AddListener(d.Move);
        OnMoveTurret.AddListener(d.HandleTurretMovement);
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
        Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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

    private void SelectionCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && (hit.collider.gameObject.GetComponent<Actor>() || hit.collider.transform.parent.GetComponent<Actor>()))
            {
                // Get the actor
                Actor a = null;

                if(hit.collider.gameObject.GetComponent<Actor>() != null)
                {
                    a = hit.collider.gameObject.GetComponent<Actor>();
                }
                else if (hit.collider.transform.parent.GetComponent<Actor>())
                {
                    a = hit.collider.transform.parent.GetComponent<Actor>();
                }

                // Not currently being controlled by someone else?
                if(a.controller == null)
                {
                    // Select it
                    SetDirectControl(a);
                }
            }
        }
    }
}
