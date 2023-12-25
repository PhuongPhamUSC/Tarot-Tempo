using UnityEngine;
using System;
public class FollowCamera : MonoBehaviour
{
    public Joystick m_joyStick;
    public GameObject m_target;
    public Vector3 m_targetOffset;
    public float m_panSpeed = 180.0f;   // degrees per second
    public float m_tiltSpeed = 180.0f;  // degrees per second
    public float m_tiltMax = 0.0f;      // degrees
    public float m_tiltMin = -90.0f;    // degrees
    float m_distanceCurrent;
    float m_distanceOriginal;
    float m_azimuth;
    float m_elevation;
    float m_followSpeed = 10.0f;

    Vector3 m_initCameraOffset;
    Vector3 m_targetPosition;

    CamInput m_camInput = new CamInput();

    public class CamInput {

    }

    // Start is called before the first frame update
    void Start()
    {
        m_initCameraOffset = transform.position - m_target.transform.position;
        CalculateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        // key board input left/right
        float azimuthAngle = m_azimuth * Mathf.Rad2Deg;
        if (Input.GetKey(KeyCode.LeftArrow) || m_joyStick.Horizontal < 0.0f) {
            azimuthAngle += m_panSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || m_joyStick.Horizontal > 0.0f) {
            azimuthAngle -= m_panSpeed * Time.deltaTime;
        }

        // make sure azimuth wrap around -180 to 180 degrees
        azimuthAngle = WrapAround180(azimuthAngle);
        m_azimuth = azimuthAngle * Mathf.Deg2Rad;
        
        // key board input up/down
        float elevationAngle = m_elevation * Mathf.Rad2Deg;
        if (Input.GetKey(KeyCode.UpArrow) || m_joyStick.Vertical > 0.0f) {
            elevationAngle += m_tiltSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || m_joyStick.Vertical < 0.0f) {
            elevationAngle -= m_tiltSpeed * Time.deltaTime;
        }
        
        // clamp elevation
        elevationAngle = Mathf.Clamp(elevationAngle, m_tiltMin, m_tiltMax);
        m_elevation = elevationAngle * Mathf.Deg2Rad;

        // check for collision between the target and the camera at original distance
        RaycastHit hit;
        Vector3 rayDirection = transform.position - m_targetPosition;
        rayDirection.Normalize();
        if (Physics.SphereCast(m_targetPosition, 0.5f, rayDirection, out hit, m_distanceOriginal)) {
            Debug.DrawLine(m_targetPosition, hit.point, Color.blue);
            m_distanceCurrent = hit.distance - 0.5f;
        }
        else {
            // back to original distance, lerping smoothly
            m_distanceCurrent = Mathf.Lerp(m_distanceCurrent, m_distanceOriginal, m_followSpeed * Time.deltaTime);
        }
    }

    void LateUpdate() {
        // calculate new location of camera and update target position
        m_targetPosition = m_target.transform.position + m_targetOffset;
        Vector3 newCameraPosition = m_targetPosition + new Vector3(m_distanceCurrent * Mathf.Sin(m_elevation) * Mathf.Cos(m_azimuth),
                                                m_distanceCurrent * Mathf.Cos(m_elevation),
                                                m_distanceCurrent * Mathf.Sin(m_elevation) * Mathf.Sin(m_azimuth));
        transform.position = Vector3.Lerp(transform.position, newCameraPosition, m_followSpeed * Time.deltaTime);
        transform.LookAt(m_targetPosition);
    }

    void CalculateCamera() {
        // calculate the position of the target based on the offset
        m_targetPosition = m_target.transform.position + m_targetOffset;
        Vector3 toTarget = m_targetPosition - transform.position;
        m_distanceCurrent = toTarget.magnitude;
        m_distanceOriginal = m_distanceCurrent;
        m_azimuth = Mathf.Atan2(toTarget.z, toTarget.x);
        m_elevation = Mathf.Acos(toTarget.y / m_distanceCurrent);
    }

    public float WrapAround180(float value)
    {
        if (value > 180.0f) {
            value -= 360.0f;
        } else if (value < -180.0f) {
            value += 360.0f;
        }
        return value;
    }
}
