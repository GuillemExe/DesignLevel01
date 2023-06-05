using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public LayerMask layerMaskCanDetect;

    public CharacterControllerV1 characterController;

    public float distance = 5.0f;
    public float sensitivity = 5.0f;
    public float sensitivityScroll = 5.0f;
    public float damping = 1.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    public float collisionOffset = 0.2f;

    private float currentDistance;

    void Start()
    {
        currentDistance = distance;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * sensitivityScroll;
    }

    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        if (Input.GetMouseButton(1))
            transform.Translate(-mouseX * Time.deltaTime, -mouseY * Time.deltaTime, 0);

        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        Vector3 desiredPosition = target.position - transform.forward * currentDistance;
        RaycastHit hit;

        if (Physics.Linecast(target.position, desiredPosition, out hit, layerMaskCanDetect))
        {
            currentDistance = hit.distance - collisionOffset;
            desiredPosition = target.position - transform.forward * currentDistance;
        }

        Vector3 offset = transform.forward * collisionOffset;
        Vector3 finalPosition = desiredPosition + offset;

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * damping);

        transform.LookAt(target);

        //float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        //float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        //if (Input.GetMouseButton(1))
        //{
        //    transform.Translate(-mouseX * Time.deltaTime, -mouseY * Time.deltaTime, 0);
        //}

        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        //currentDistance -= scroll * sensitivityScroll;

        //currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        //Vector3 desiredPosition = target.position - transform.forward * currentDistance;
        //RaycastHit hit;

        //if (Physics.Linecast(target.position, desiredPosition, out hit, layerMaskCanDetect))
        //{
        //    currentDistance = hit.distance - collisionOffset;
        //}

        //transform.position = Vector3.Lerp(transform.position, target.position - transform.forward * currentDistance, Time.deltaTime * damping);

        //transform.LookAt(target);
    }
}
