using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{

    public Transform cameraPosition;
    public float followSharpness = 35f;


    void LateUpdate()
    {
        if (cameraPosition == null)
        {
            return;
        }

        float blend = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, cameraPosition.position, blend);
    }
}
