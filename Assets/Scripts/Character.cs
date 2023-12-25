using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Joystick m_joyStick;
    public float m_walkSpeed = 4.0f;
    public float m_turnSpeed = 360.0f;  // degrees per second
    CharacterController m_characterController;
    CharInput m_charInput = new CharInput();
    Animator m_animator;
    AudioSource m_stepAudio;
    float m_stepVolume;

    public class CharInput {
        public Vector3 m_moveDirection = Vector3.zero;
        public float m_facingDirection = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_stepAudio = GetComponent<AudioSource>();
        m_stepVolume = m_stepAudio.volume;
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponent<CharacterController>();   
    }

    // Update is called once per frame
    void Update()
    {
        m_charInput.m_facingDirection = Camera.main.transform.eulerAngles.y;
        // key board and joystick input forward/backward
        Vector3 rightVec = Vector3.zero;
        if (Input.GetKey(KeyCode.W) || m_joyStick.Vertical > 0.0f)
        {
            rightVec = Camera.main.transform.forward.normalized;
        }
        else if (Input.GetKey(KeyCode.S) || m_joyStick.Vertical < 0.0f)
        {
            rightVec = -Camera.main.transform.forward.normalized;
        }
        if (Mathf.Abs(m_joyStick.Vertical) > Mathf.Epsilon)
        {
            rightVec *= Mathf.Abs(m_joyStick.Vertical);
        }

        // key board input left/right
        Vector3 leftVec = Vector3.zero;
        if (Input.GetKey(KeyCode.A) || m_joyStick.Horizontal < 0.0f) {
            leftVec = -Camera.main.transform.right.normalized;
        }
        else if (Input.GetKey(KeyCode.D) || m_joyStick.Horizontal > 0.0f) {
            leftVec = Camera.main.transform.right.normalized;
        }
        if (Mathf.Abs(m_joyStick.Horizontal) > Mathf.Epsilon)
        {
            leftVec *= Mathf.Abs(m_joyStick.Horizontal);
        }

        // if none of the key is clicked, reset to zero
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && m_joyStick.Horizontal == 0.0f && m_joyStick.Vertical == 0.0f) {
            m_charInput.m_moveDirection = Vector3.zero;
            if (m_stepAudio != null && m_stepAudio.isPlaying)
                StartCoroutine(FadeOut());
        } 
        else if (m_stepAudio != null && !m_stepAudio.isPlaying)
        {
            m_stepAudio.volume = m_stepVolume;
            m_stepAudio.Play();
        }

        // normalize direction
        m_charInput.m_moveDirection = leftVec + rightVec;
        if (m_charInput.m_moveDirection.magnitude > 1.0f) {
            m_charInput.m_moveDirection.Normalize();
        }
        // movement
        m_characterController.SimpleMove(m_charInput.m_moveDirection * m_walkSpeed);

        // convert the world-space input controls into object-space
        Vector3 localMoveDirection = transform.InverseTransformDirection(m_charInput.m_moveDirection);
        m_animator.SetFloat("ForwardBack", localMoveDirection.z);
        m_animator.SetFloat("RightLeft", localMoveDirection.x);

        // rotation
        float currentAngle = transform.eulerAngles.y;
        float targetAngle = m_charInput.m_facingDirection;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);
        float turnAmount = m_turnSpeed * Time.deltaTime;
        if (Mathf.Abs(angleDifference) > turnAmount)
        {
            if (angleDifference < 0.0f)
            {
                transform.Rotate(0.0f, -turnAmount, 0.0f, Space.Self);
            }
            else
            {
                transform.Rotate(0.0f, turnAmount, 0.0f, Space.Self);
            }
        }
        else
        {
            transform.Rotate(0.0f, angleDifference, 0.0f, Space.Self);
        }

    }

    private System.Collections.IEnumerator FadeOut()
    {
        float startVolume = m_stepAudio.volume;

        while (m_stepAudio.volume > 0)
        {
            m_stepAudio.volume -= startVolume * Time.deltaTime / 1f;
            yield return null;
        }

        // Pause or stop the audio source once faded out
        m_stepAudio.Stop();
    }
}
