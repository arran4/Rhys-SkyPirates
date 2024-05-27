using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 Forward { get; private set; }
    public Vector3 Right { get; private set; }

    private float movementSpeed;
    public float movementSpeedNormal;
    public float movementSpeedFast;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    void Update()
    {
        Forward = transform.forward;
        Right = new Vector3(Forward.z, 0, -Forward.x);
        movementSpeed = (newZoom.y >= 325) ? movementSpeedFast : movementSpeedNormal;
    }

    public void Movement(Vector2 move)
    {
        newPosition += transform.forward * move.y * movementSpeed + transform.right * move.x * movementSpeed;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    public void Rotation(float rotate)
    {
        newRotation *= Quaternion.Euler(Vector3.up * rotate * rotationAmount);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    public void Zoom(float zoom)
    {
        newZoom += zoom * zoomAmount;
        newZoom.y = Mathf.Clamp(newZoom.y, 50f, 500f);
        newZoom.z = Mathf.Clamp(newZoom.z, -500f, -50f);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}