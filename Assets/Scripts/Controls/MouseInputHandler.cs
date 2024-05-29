using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputHandler : MonoBehaviour
{
    private CameraController cameraController;
    private float holdTimer = 0;
    private Vector3 dragStartPosition;
    private Vector3 rotateStartPosition;
    private BasicControls inputActions;
    public float Sensitivity;

    void Start()
    {
        cameraController = GetComponent<CameraController>();
        inputActions = EventManager.EventInstance.inputActions;
    }

    void Update()
    {
        HandleMouseMovement();
        HandleMouseRotation();
        cameraController.Movement(inputActions.Battle.MoveCamera.ReadValue<Vector2>());
        cameraController.Rotation(inputActions.Battle.RotateCamera.ReadValue<float>());
        cameraController.Zoom(inputActions.Battle.Zoom.ReadValue<float>());
    }

    private void HandleMouseMovement()
    {
        if (Mouse.current.leftButton.IsPressed())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                dragStartPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                holdTimer += Time.deltaTime;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame && holdTimer > 0.25f)
            {
                Vector3 dragCurrentPosition = Mouse.current.position.ReadValue();
                Vector2 movement = dragCurrentPosition - dragStartPosition;
                movement = movement.normalized; //Correct for extream numbers back to a reasonable move
                cameraController.Movement(-movement / Sensitivity);
                dragStartPosition = dragCurrentPosition;
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
                rotateStartPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                holdTimer += Time.deltaTime;
            }

            if (!Mouse.current.rightButton.wasPressedThisFrame && holdTimer > 0.25f)
            {
                Vector3 rotateCurrentPosition = Mouse.current.position.ReadValue();
                Vector3 difference = rotateStartPosition - rotateCurrentPosition;
                cameraController.Rotation(-(difference.x - difference.y) / Sensitivity);
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
}
