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

    CharacterController oCharacterController;

    public float fSpeed = 6.0f;
    public float fRunningSpeed = 3.0f;
    public float fWalkingSpeed = .5f;
    public float fStandardSpeed = 1.0f;
    public float fJumpSpeed = 8.0f;
    public float fGravity = 20.0f;
    public float fRotSpeed = 90;
    public float fCrouchDeltaHeight = 0.15f;
    public const float fMaxStamina = 10.0f;
    public float fStamina = fMaxStamina;
    public float fStaminaRecoveryRate = 5f;
    public float fStaminaSpendingRate = 1f;
    public bool bTired = false;
    delegate void OnMovementStateChangeDelegate(CharacterMovementState _eOldState, CharacterMovementState _eNewState);
    event System.Action<CharacterMovementState, CharacterMovementState> OnMovementStateChange;

    CharacterMovementState eState = CharacterMovementState.Default;


    private Vector3 vMoveDirection = Vector3.zero;

    void Start()
    {
        oCharacterController = GetComponent<CharacterController>();
        AddToMovementStateChangeCaller(OnMovementStateChanged);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !bTired)
        {
            if (eState != CharacterMovementState.Running)
            {
                fSpeed = fRunningSpeed;
                setMovementState(CharacterMovementState.Running);
            }
            fStamina -= fStaminaSpendingRate * Time.deltaTime;
            if (fStamina <= 0)
            {
                bTired = true;
            }

        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (eState != CharacterMovementState.Walking)
            {
                fSpeed = fWalkingSpeed;
                setMovementState(CharacterMovementState.Walking);
            }
        }
        else if (eState != CharacterMovementState.Default || (fStamina <= 0 && eState == CharacterMovementState.Running))
        {
            fSpeed = fStandardSpeed;
            setMovementState(CharacterMovementState.Default);
        }

        if (eState != CharacterMovementState.Running && fStamina <= fMaxStamina)
        {
            fStamina += fStaminaRecoveryRate * Time.deltaTime;
            if (fStamina > fMaxStamina)
            {
                fStamina = fMaxStamina;
                bTired = false;
            }
        }

        if (oCharacterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            vMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            vMoveDirection = transform.TransformDirection(vMoveDirection);
            vMoveDirection *= fSpeed;

            if (Input.GetButton("Jump"))
            {
                vMoveDirection.y = fJumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        vMoveDirection.y -= fGravity * Time.deltaTime;

        // Move the controller
        oCharacterController.Move(vMoveDirection * Time.deltaTime);
    }


    CharacterMovementState getMovementState()
    {
        return eState;
    }
    void setMovementState(CharacterMovementState _eNewState)
    {
        if (eState == _eNewState) return;
        OnMovementStateChange(eState, _eNewState);
        eState = _eNewState;
    }

    void OnMovementStateChanged(CharacterMovementState _eOldState, CharacterMovementState _eNewState)
    {
        switch (_eNewState)
        {
            case CharacterMovementState.Default:
                if (_eOldState == CharacterMovementState.Walking)
                {
                    GetComponent<CharacterController>().radius += fCrouchDeltaHeight;
                    //GetComponent<BoxCollider>().center += new Vector3(0, fCrouchDeltaHeight / 2, 0);
                }
                break;
            case CharacterMovementState.Running:
                if (_eOldState == CharacterMovementState.Walking)
                {
                    GetComponent<CharacterController>().radius += fCrouchDeltaHeight;
                    //GetComponent<BoxCollider>().center += new Vector3(0, fCrouchDeltaHeight / 2, 0);
                }
                break;
            case CharacterMovementState.Walking:
                GetComponent<CharacterController>().radius -= fCrouchDeltaHeight;
                //GetComponent<BoxCollider>().center -= new Vector3(0, fCrouchDeltaHeight / 2, 0);
                break;
        }
    }

    public void AddToMovementStateChangeCaller(System.Action<CharacterMovementState, CharacterMovementState> _oFunc)
    {
        OnMovementStateChange += _oFunc;
    }
}
