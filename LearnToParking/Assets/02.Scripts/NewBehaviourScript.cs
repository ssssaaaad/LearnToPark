using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float i = 10;
    public Transform a;
    // Update is called once per frame
    void Update()
    {
        Debug.Log(Mathf.Abs((transform.rotation * a.rotation).y));
    }

}
