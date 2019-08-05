using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    RotationAxes axes = RotationAxes.MouseXAndY;
    public float fSensitivityX = 15F;
    public float fSensitivityY = 15F;
    public float fSensitivityZ = 2F;

    public float fMinimumX = -360F;
    public float fMaximumX = 360F;

    public float fMinimumY = -60F;
    public float fMaximumY = 60F;

    public float fMinimumZ = -45F;
    public float fMaximumZ = 45F;

    float fRotationY = 0F;
    float fRotationZ = 0F;
    float frotationX = 0F;

    public float fReturnRate = 1.0f;
    public bool bRestoreRotation = false;
    void Update()
    {
        if ((Input.GetAxis("Incline") != 0))
        {
            fRotationZ += (Input.GetAxis("Incline")) * fSensitivityZ;
            fRotationZ = Mathf.Clamp(fRotationZ, fMinimumZ, fMaximumZ);
            bRestoreRotation = false;
        }
        else
        {
            bRestoreRotation = true;
        }

        if (axes == RotationAxes.MouseXAndY)
        {
            frotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * fSensitivityX;
            fRotationY += Input.GetAxis("Mouse Y") * fSensitivityY;
            fRotationY = Mathf.Clamp(fRotationY, fMinimumY, fMaximumY);
        }

        if (bRestoreRotation)
        {
            fRotationZ = Mathf.Lerp(fRotationZ, 0, Time.deltaTime * fReturnRate);
            if (fRotationZ == 0)
            {
                bRestoreRotation = false;
            }
            transform.localEulerAngles = new Vector3(-fRotationY, frotationX, fRotationZ);
        }
        else
        {
            transform.localEulerAngles = new Vector3(-fRotationY, frotationX, fRotationZ);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

}
