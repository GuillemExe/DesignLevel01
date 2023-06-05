using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnObject : MonoBehaviour
{
    public float rotationSpeed = 1f;
    private bool isRotating = false;
    public bool onTarget = false;

    private void Update()
    {
        if (onTarget)
        {
            if (!isRotating)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    StartRotation(Vector3.forward);
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    StartRotation(Vector3.back);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    StartRotation(Vector3.left);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    StartRotation(Vector3.right);
                }
            }
        }
    }

    private void StartRotation(Vector3 direction)
    {
        StartCoroutine(RotateObject(direction));
    }

    private IEnumerator RotateObject(Vector3 direction)
    {
        isRotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(direction * 90f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
}
