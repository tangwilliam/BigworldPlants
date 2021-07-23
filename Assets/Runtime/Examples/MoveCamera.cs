using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float Speed = 1.0f;

    Transform m_CameraTrans;

    // Start is called before the first frame update
    void Start()
    {
        m_CameraTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaDistance = Time.deltaTime * Speed;
        Vector3 oldPosition = m_CameraTrans.transform.position;
        m_CameraTrans.transform.position = new Vector3(oldPosition.x + deltaDistance, oldPosition.y, oldPosition.z);
    }
}
