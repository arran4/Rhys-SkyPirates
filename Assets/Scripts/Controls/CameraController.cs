using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 Forward;
    public Vector3 Right;

    private float movementSpeed;
    public float movementSpeedNormal;
    public float movementSpeedFast;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 rotateStartPosition;
    private float holdTimer = 0;

    public BasicControls inputActions;

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        inputActions = EventManager.EventInstance.inputActions;
    }

    void Update()
    {
        Forward = transform.forward;
        Right = new Vector3(Forward.z, 0, -Forward.x);
        movementSpeed = (newZoom.y >= 325) ? movementSpeedFast : movementSpeedNormal;

        HandleMouseInput();
        Movement(inputActions.Battle.MoveCamera.ReadValue<Vector2>());
        Rotation(inputActions.Battle.RotateCamera.ReadValue<float>());
        Zoom(inputActions.Battle.Zoom.ReadValue<float>());
    }

    private void HandleMouseInput()
    {
        HandleMouseMovement();
        HandleMouseRotation();
    }
    private void HandleMouseMovement()
    {
        if (Mouse.current.leftButton.IsPressed())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                dragStartPosition = GetMouseWorldPosition();
            }
            else
            {
                holdTimer += Time.deltaTime;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame && holdTimer > 0.25f)
            {
                Vector3 dragCurrentPosition = GetMouseWorldPosition();
                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && holdTimer < 0.25f)
        {
            RaycastHit hit;
            if (Physics.Raycast(GetMouseRay(), out hit, 100000f))
            {
                EventManager.TileSelectTrigger(hit.transform.gameObject);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            holdTimer = 0;
        }
    }

    private void HandleMouseRotation()
    {
        if (Mouse.current.rightButton.IsPressed())
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                rotateStartPosition = GetMouseWorldPosition();
            }
            else
            {
                holdTimer += Time.deltaTime;
            }

            if (!Mouse.current.rightButton.wasPressedThisFrame && holdTimer > 0.25f)
            {
                Vector3 rotateCurrentPosition = GetMouseWorldPosition();
                Vector3 difference = rotateStartPosition - rotateCurrentPosition;
                newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
                rotateStartPosition = rotateCurrentPosition;
            }
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame && holdTimer < 0.25f)
        {
            EventManager.TileDeselectTrigger();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            holdTimer = 0;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        float entry;
        if (plane.Raycast(ray, out entry))
        {
            return ray.GetPoint(entry);
        }
        return Vector3.zero;
    }

    private Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

     private void Movement(Vector2 move)
     {
        newPosition += transform.forward * move.y * movementSpeed + transform.right * move.x * movementSpeed;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
     }
    

    private void Rotation(float rotate)
    {
        newRotation *= Quaternion.Euler(Vector3.up * rotate * rotationAmount);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    private void Zoom(float zoom)
    {
        newZoom += zoom * zoomAmount;

        newZoom.y = Mathf.Clamp(newZoom.y, 50f, 500f);
        newZoom.z = Mathf.Clamp(newZoom.z, -500f, -50f);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}