using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    public Vector3 Velocity = new Vector3(2.0f, 0.0f, 0.0f);


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position +=  Time.deltaTime * Velocity;
    }
}
