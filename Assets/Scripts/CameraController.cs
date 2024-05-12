using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//ripped from another project will really need to fix. temporay solution I am not interested in coding right now.
public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    private float movementSpeed;
    public float movementSpeedNormal;
    public float movementSpeedFast;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;
    public float HoldTimer = 0;

    public BasicControls inputActions; 
    // Start is called before the first frame update

    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        inputActions = EventManager.EventInstance.inputActions;
    }

    // Update is called once per frame
    void Update()
    {
        if (newZoom.y >=325)
        {
            movementSpeed = movementSpeedFast;
        }
        else
        {
            movementSpeed = movementSpeedNormal;
        }
        HandleMouseMovement();
        HandleMouseRotate();
        Movement(inputActions.Battle.MoveCamera.ReadValue<Vector2>());
        Rotation(inputActions.Battle.RotateCamera.ReadValue<float>());
        Zoom(inputActions.Battle.Zoom.ReadValue<float>());
       
    }


    public void HandleMouseRotate()
    {
        if (Mouse.current.rightButton.IsPressed() && Mouse.current.rightButton.wasPressedThisFrame)
        {
            rotateStartPosition = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y,0);
        }
        else if (Mouse.current.rightButton.IsPressed())
        {
            HoldTimer += Time.deltaTime;
        }

        if (Mouse.current.rightButton.IsPressed() && !Mouse.current.rightButton.wasPressedThisFrame && HoldTimer > 0.25f)
        {
            rotateCurrentPosition = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            //reset the drag position for the next frame
            rotateStartPosition = rotateCurrentPosition;

            //Negating the value here so that the world spins in the opposite direction of the drag 

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
        }
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            HoldTimer = 0;
        }
    }

    public void HandleMouseMovement()
    {
        if (Mouse.current.leftButton.IsPressed() && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x ,Mouse.current.position.ReadValue().y, 0f));
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        else if(Mouse.current.leftButton.IsPressed())
        {
            HoldTimer += Time.deltaTime;
        }
        
        if (Mouse.current.leftButton.IsPressed() && !Mouse.current.leftButton.wasPressedThisFrame && HoldTimer > 0.25f)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0f));
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HoldTimer = 0;
        }
    }

    public void Movement(Vector2 Move)
    {

        if (Move.y > 0)
        {
            newPosition += (transform.forward * movementSpeed);
        }
        if (Move.y < 0)
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        if (Move.x > 0)
        {
            newPosition += (transform.right * movementSpeed);
        }
        if (Move.x < 0)
        {
            newPosition += (transform.right * -movementSpeed);
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
    }

    public void Rotation(float Rotate)
    {

        if (Rotate > 0)
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Rotate < 0)
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }
    public void Zoom(float Zoom)
    {

        if (Zoom > 0)
        {
            newZoom += zoomAmount;
            if (newZoom.y < 50)
            {
                newZoom.y = 50;
            }
            if (newZoom.z > -50)
            {
                newZoom.z = -50;
            }
            if (newZoom.y > 700)
            {
                newZoom.y = 700;
            }
            if (newZoom.z < -700)
            {
                newZoom.z = -700;
            }

        }
        if (Zoom < 0)
        {
            newZoom -= zoomAmount;

            if (newZoom.y < 50)
            {
              newZoom.y = 50;
            }
            if (newZoom.z > -50)
            {
                newZoom.z = -50;
            }
            if (newZoom.y >700)
            {
                newZoom.y = 700;
            }
            if (newZoom.z < -700)
            {
                newZoom.z = -700;
            }

           
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}


