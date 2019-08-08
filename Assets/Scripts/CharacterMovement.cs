using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public enum CharacterMovementState : uint
    {
        Default = 0x0001,
        Running = 0x0010,
        Walking = 0x0100,
    }

    CharacterController m_oCharacterController;
    CharacterRotation m_oCharacterRotation;

    public float m_fSpeed = 1.0f;
    public float m_fRunningSpeed = 3.0f;
    public float m_fWalkingSpeed = .5f;
    public float m_fStandardSpeed = 1.0f;
    public float m_fJumpSpeed = 8.0f;
    public float m_fGravity = 20.0f;
    public float m_fRotSpeed = 90;
    public float m_fCrouchDeltaHeight = 0.15f;
    public const float m_fMaxStamina = 10.0f;
    public float m_fStamina = m_fMaxStamina;
    public float m_fStaminaRecoveryRate = 5f;
    public float m_fStaminaSpendingRate = 1f;
    public bool m_bTired = false;
    public float m_fQuantityToMoveWhenInclined = 0.1f;

    float m_fMovementQuantityByInclination = 0.2f;
    delegate void OnMovementStateChangeDelegate(CharacterMovementState _eOldState, CharacterMovementState _eNewState);
    event System.Action<CharacterMovementState, CharacterMovementState> m_evtOnMovementStateChange;
    CharacterMovementState eState = CharacterMovementState.Default;
    float m_fPositionZ;
    float m_fInclinedTime;
    Coroutine m_fInclineCoroutine = null;

    private Vector3 vMoveDirection = Vector3.zero;

    void Start()
    {
        m_oCharacterController = GetComponent<CharacterController>();
        m_oCharacterRotation = GetComponent<CharacterRotation>();
        AddToMovementStateChangeCaller(m_evtOnMovementStateChanged);
        m_fInclinedTime = m_oCharacterRotation.m_fInclinedTime;
    }

    void Update()
    {
        if ((Input.GetAxis("Incline") != 0))
        {
            return;
        }
        else
        {

        }
        if (Input.GetKey(KeyCode.LeftShift) && !m_bTired)
        {
            if (eState != CharacterMovementState.Running)
            {
                m_fSpeed = m_fRunningSpeed;
                setMovementState(CharacterMovementState.Running);
            }
            m_fStamina -= m_fStaminaSpendingRate * Time.deltaTime;
            if (m_fStamina <= 0)
            {
                m_bTired = true;
            }

        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (eState != CharacterMovementState.Walking)
            {
                m_fSpeed = m_fWalkingSpeed;
                setMovementState(CharacterMovementState.Walking);
            }
        }
        else if (eState != CharacterMovementState.Default || (m_fStamina <= 0 && eState == CharacterMovementState.Running))
        {
            m_fSpeed = m_fStandardSpeed;
            setMovementState(CharacterMovementState.Default);
        }

        if (eState != CharacterMovementState.Running && m_fStamina <= m_fMaxStamina)
        {
            m_fStamina += m_fStaminaRecoveryRate * Time.deltaTime;
            if (m_fStamina > m_fMaxStamina)
            {
                m_fStamina = m_fMaxStamina;
                m_bTired = false;
            }
        }

        if (m_oCharacterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            vMoveDirection = transform.TransformDirection(vMoveDirection);
            vMoveDirection *= m_fSpeed;

            if (Input.GetButton("Jump"))
            {
                vMoveDirection.y = m_fJumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        vMoveDirection.y -= m_fGravity * Time.deltaTime;

        // Move the controller
        m_oCharacterController.Move(vMoveDirection * Time.deltaTime);
    }


    CharacterMovementState getMovementState()
    {
        return eState;
    }
    void setMovementState(CharacterMovementState _eNewState)
    {
        if (eState == _eNewState) return;
        m_evtOnMovementStateChange(eState, _eNewState);
        eState = _eNewState;
    }

    void m_evtOnMovementStateChanged(CharacterMovementState _eOldState, CharacterMovementState _eNewState)
    {
        switch (_eNewState)
        {
            case CharacterMovementState.Default:
                if (_eOldState == CharacterMovementState.Walking)
                {
                    GetComponent<CharacterController>().radius += m_fCrouchDeltaHeight;
                    //GetComponent<BoxCollider>().center += new Vector3(0, m_fCrouchDeltaHeight / 2, 0);
                }
                break;
            case CharacterMovementState.Running:
                if (_eOldState == CharacterMovementState.Walking)
                {
                    GetComponent<CharacterController>().radius += m_fCrouchDeltaHeight;
                    //GetComponent<BoxCollider>().center += new Vector3(0, m_fCrouchDeltaHeight / 2, 0);
                }
                break;
            case CharacterMovementState.Walking:
                GetComponent<CharacterController>().radius -= m_fCrouchDeltaHeight;
                //GetComponent<BoxCollider>().center -= new Vector3(0, m_fCrouchDeltaHeight / 2, 0);
                break;
        }
    }

    public void AddToMovementStateChangeCaller(System.Action<CharacterMovementState, CharacterMovementState> _oFunc)
    {
        m_evtOnMovementStateChange += _oFunc;
    }

    private IEnumerator MoveToInclination(float _fPositionToMove, float _fTimeToRotate)
    {
        float fPercentage = 0;
        Vector3 vDir;
        if (_fPositionToMove > 0)
            vDir = transform.right;
        else
            vDir = -transform.right;
        while (fPercentage < 1)
        {
            fPercentage += Time.deltaTime / m_fInclinedTime;

            yield return null;
        }
        m_fInclineCoroutine = null;
    }
}
