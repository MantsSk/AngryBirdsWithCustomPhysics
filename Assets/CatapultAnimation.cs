using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultAnimation : MonoBehaviour
{
    int throwValue;
    float zAxis;

    // Start is called before the first frame update
    void Start()
    {
        throwValue = 0;
        zAxis = 20.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) 
        {
            if (throwValue < 20) 
            {
                zAxis += 1.8f;
                transform.localRotation = Quaternion.Euler(19f, 22.5f, zAxis);
                throwValue += 1;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow)) 
        {
            transform.localRotation = Quaternion.Euler(19f, 22.5f, 24.29f);
        }
    }
}
