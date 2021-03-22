using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    public DynamicJoystick dynamicJoystick;
    public Rigidbody rb;

    public void Update()
    {
        /*if (dynamicJoystick.Vertical != 0 && dynamicJoystick.Horizontal != 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            // walkingForward = true;
            rb.constraints = RigidbodyConstraints.None;

            float value =
                (float) ((Mathf.Atan2(dynamicJoystick.Horizontal, dynamicJoystick.Vertical) / Mathf.PI) * 180f);
            if (value < 0) value += 360f;
//            Debug.Log((rb.rotation.normalized.eulerAngles.z-90f)/180f+"    "+(Vector3.forward * dynamicJoystick.Vertical + Vector3.right * dynamicJoystick.Horizontal).normalized.z);
            var delta = Clamp0360(rb.rotation.normalized.eulerAngles.y) - value;
            int sign = 0;
            if (Mathf.Abs(delta) < 180f) sign = -1 * (int) Mathf.Sign(delta);
            else sign = (int) Mathf.Sign(delta);
//            rb.AddRelativeTorque(
//                0, (Clamp0360(rb.rotation.normalized.eulerAngles.y) - value) / 360f *
//                   400 * Time.deltaTime, 0);

            delta = Mathf.Abs(delta);
            if (delta > 180) delta = 360 - delta;

            Debug.Log(delta);
            rb.AddTorque(transform.up * (delta / 180f) * sign * 1000 * Time.deltaTime);
            /*  rb.transform.localRotation = Quaternion.LookRotation( 
                      new Vector3(dynamicJoystick.Horizontal, 0f, dynamicJoystick.Vertical),
                      Vector3.up)#1#
            ; //w   SteppySteps('L');
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        if (dynamicJoystick.Vertical == 0 && dynamicJoystick.Horizontal == 0)
        {
            //  walkingForward = false;
        }*/
    }

    private float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }

        return result;
    }
}