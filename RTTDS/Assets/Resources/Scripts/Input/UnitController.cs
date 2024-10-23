using UnityEngine;
using System.Resources;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using TMPro;

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

    private void Awake()
    {
        // Change this later if needed
        mainCamera = Camera.main;
        camTargetPos = mainCamera.transform.position;
        camTargetZoom = mainCamera.orthographicSize;
    }

    public void Update()
    {
        SelectionCheck();
        CameraControl();
        if (directControl != null)
        {
            CheckDirectInput();
        }
    }

    #region Utilities
    private bool followDC = false;

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

        // Apply vision prefab
	    // (The original vision shaders & setup is by 'aarthificial': https://www.youtube.com/watch?v=XWMPEE8O05c)
        if(d.vision_component == null)
        {
            GameObject vision = Instantiate(GameManager.inst.prefab_vision, d.transform.position, Quaternion.identity);
            vision.transform.parent = d.transform;
            d.vision_component = vision;
        }

        // Re-locate camera
        StartCoroutine(RelocateCameraSmooth(d.transform));
        followDC = true;
    }

    private IEnumerator RelocateCameraSmooth(Transform t)
    {
        Vector3 targetPosition = t.position + new Vector3(0, 0, mainCamera.transform.position.z); // Target position based on player's position and offset
        float timeElapsed = 0;

        // Smoothly move the camera to the player's position
        while (timeElapsed < camSmoothMove)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, timeElapsed / camSmoothMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
    }

    public void DropDirectControl(Actor d)
    {
        d.controller = null;
        Destroy(d.vision_component); // Change this later?
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

        // Always allow the player to detach the camera
        if (followDC)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                followDC = false;
            }

            // If not, follow the controlled unit
            mainCamera.transform.position = p.transform.position + new Vector3(0, 0, mainCamera.transform.position.z);
        }
    }

    private void InputBody(Actor p)
    {
        Vector2 movementVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) // Move Up
            movementVector.y += 1;
        if (Input.GetKey(KeyCode.S)) // Move Down
            movementVector.y -= 1;
        if (Input.GetKey(KeyCode.A)) // Move Left
            movementVector.x -= 1;
        if (Input.GetKey(KeyCode.D)) // Move Right
            movementVector.x += 1; 
        
        OnMoveBody?.Invoke(movementVector.normalized); // Links to --> Move(Vector2)
    }

    private void InputTurret(Actor p)
    {
        // TODO: Check if the player should be allowed to move the turret
        if (Input.GetMouseButton(1)) // Hold down right click
        {
            OnMoveTurret?.Invoke(GetMousePosition());
        }
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

    #region Misc Input
    private void SelectionCheck()
    {
        // Left click but not right clicking
        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
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
                else if(a.controller == this) // Or we are currently controlling it
                { // Re-enable following with camera (maybe do more here later)
                    followDC = true;
                }
            }
        }
    }

    [Header("Camera")]
    public Camera mainCamera;
    private float camSmoothMove = 0.125f;
    private float camMoveSpeed = 5f;
    private float camZoomSpeed = 5f;
    private Vector2 zoomLimits = new Vector2(3, 10);
    private Vector2 panLimits = new Vector2(-50, 50);
    [SerializeField] private bool smoothCamMove = false;
    [SerializeField] private bool smoothCamZoom = false;
    //
    private Vector3 camTargetPos;
    private float camTargetZoom;

    private void CameraControl()
    {
        if (!followDC)
        {
            // Camera move input
            if (Input.GetKey(KeyCode.UpArrow))
            {
                camTargetPos += new Vector3(0, camMoveSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                camTargetPos += new Vector3(0, -camMoveSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                camTargetPos += new Vector3(-camMoveSpeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                camTargetPos += new Vector3(camMoveSpeed * Time.deltaTime, 0, 0);
            }

            // Restrict camera position
            float clampedX = Mathf.Clamp(camTargetPos.x, panLimits.x, panLimits.y);
            float clampedY = Mathf.Clamp(camTargetPos.y, panLimits.x, panLimits.y);
            camTargetPos = new Vector3(clampedX, clampedY, camTargetPos.z);

            // Move the Camera
            if (smoothCamMove)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, camTargetPos, Time.deltaTime * camMoveSpeed);
            }
            else
            {
                mainCamera.transform.position = camTargetPos;
            }

            // Zoom the camera
            float zoomChange = Input.GetAxis("Mouse ScrollWheel") * camZoomSpeed;
            camTargetZoom = Mathf.Clamp(camTargetZoom - zoomChange, zoomLimits.x, zoomLimits.y);

            if (smoothCamZoom)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, camTargetZoom, Time.deltaTime * camZoomSpeed);
            }
            else
            {
                mainCamera.orthographicSize = camTargetZoom;
            }
        }
    }
    #endregion
}
