using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    RotationAxes m_eAxes = RotationAxes.MouseXAndY;
    public float m_fSensitivityX = 5F;
    public float m_fSensitivityY = 5F;
    public float m_fSensitivityZ = 2F;

    public float m_fMinimumX = -360F;
    public float m_fMaximumX = 360F;

    public float m_fMinimumY = -60F;
    public float m_fMaximumY = 60F;

    public float m_fMinimumZ = -45F;
    public float m_fMaximumZ = 45F;

    float m_fRotationY = 0F;
    float m_fRotationZ = 0F;
    float m_fRotationX = 0F;

    public float m_fReturnRate = 1.0f;
    public float m_fInclinedTime = 0.0f;
    bool m_bRestoreRotation = false;

    [HideInInspector]
    public bool m_bInclined = false;

    float m_fLerpPercentage = 0.0f;

    Coroutine m_fInclineCoroutine = null;
    CharacterMovement m_oMovementComponent;

    void Update()
    {
        if ((Input.GetAxis("Incline") != 0))
        {
            if (!m_bInclined)
            {
                if (m_fInclineCoroutine != null)
                {
                    StopCoroutine(m_fInclineCoroutine);
                }
                m_fInclineCoroutine = null;
                m_fInclineCoroutine = StartCoroutine(InclineTo(m_fMaximumZ * Input.GetAxis("Incline"), m_fInclinedTime));
                m_bInclined = true;
                m_oMovementComponent.m_bInclined = true;
            }
        }
        else
        {
            if (m_bInclined)
            {
                if (m_fInclineCoroutine != null)
                    StopCoroutine(m_fInclineCoroutine);
                m_fInclineCoroutine = null;
                m_fInclineCoroutine = StartCoroutine(InclineTo(0, m_fInclinedTime));
                m_bInclined = false;
                m_oMovementComponent.m_bInclined = false;
            }
        }
        if (!m_bInclined)
        {
            if (m_eAxes == RotationAxes.MouseXAndY)
            {
                m_fRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_fSensitivityX;
                m_fRotationY += Input.GetAxis("Mouse Y") * m_fSensitivityY;
                m_fRotationY = Mathf.Clamp(m_fRotationY, m_fMinimumY, m_fMaximumY);
            }
        }

        transform.localEulerAngles = new Vector3(-m_fRotationY, m_fRotationX, m_fRotationZ);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_oMovementComponent = GetComponent<CharacterMovement>();
    }

    private IEnumerator InclineTo(float _fAnglesToRotate, float _fTimeToRotate)
    {
        float fLerpPercentage = 0;
        while (fLerpPercentage < 1)
        {
            fLerpPercentage += Time.deltaTime / m_fInclinedTime;
            m_fRotationZ = Mathf.Lerp(m_fRotationZ, _fAnglesToRotate, fLerpPercentage);
            yield return null;
        }
        m_fInclineCoroutine = null;
    }

}
