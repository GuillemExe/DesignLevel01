using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;
using static UnityEditor.PlayerSettings;

public class DisableObstructiveMesh : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
            MakeYourTransparency(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
            MakeYourTransparency(other.gameObject, false);
    }

    private void MakeYourTransparency(GameObject gameObject, bool enter)
    {
        if (gameObject?.GetComponent<MeshRenderer>())
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (enter)
            {
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                renderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }
}
