using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    IInput m_Input;

    // Start is called before the first frame update
    void Start()
    {
        m_Input = GetComponent<IInput>();
        m_Input.OnMovementDirectionInput += (m_Input) =>
        {
            Debug.Log("Direction " + m_Input);
        };
        m_Input.OnMovementInput += (m_Input) =>
        {
            Debug.Log("Movement input " + m_Input);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
