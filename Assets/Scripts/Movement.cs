using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fHorizontalMovement = Input.GetAxis("Horizontal");
        float fVerticalMovement = Input.GetAxis("Vertical");

        if (fHorizontalMovement != 0 || fVerticalMovement != 0)
        {
            transform.Translate(fHorizontalMovement,0,fVerticalMovement);
        }
    }
}
