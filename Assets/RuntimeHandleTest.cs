using System.Collections;
using System.Collections.Generic;
using RuntimeHandle;
using UnityEngine;

public class RuntimeHandleTest : MonoBehaviour
{
    public Transform cube;
    
    // Start is called before the first frame update
    void Start()
    {
        RuntimeTransformHandle.Create(cube, HandleType.POSITION);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
