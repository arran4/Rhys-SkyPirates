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
        Movement(inputActions.Battle.MoveCamera.ReadValue<Vector2>());
        Rotation(inputActions.Battle.RotateCamera.ReadValue<float>());
        Zoom(inputActions.Battle.Zoom.ReadValue<float>());
       
    }

    void HandleMouseInput()
    {
        if (1 != 0)
        {
            //multiply the zoom amount by 2 to make zooming on the mouse feel faster
            newZoom +=0 * (zoomAmount *2);     
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
        if (true)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (true)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(0, 0, 0));
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);

                newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
        if (true)
        {
            rotateStartPosition = new Vector3 (0,0,0);
        }
        if (true)
        {
            rotateCurrentPosition = new Vector3(0, 0, 0);
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;

            //reset the drag position for the next frame
            rotateStartPosition = rotateCurrentPosition;

            //Negating the value here so that the world spins in the opposite direction of the drag 

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
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


