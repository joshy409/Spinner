using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{

    public float spinSpeed = 3600f;
    public bool doSpin = false;
    private Rigidbody rb;
    public GameObject playerGraphics;

    // Update is called once per frame
    void Update()
    {
        if (doSpin)
        {
            playerGraphics.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }
}
