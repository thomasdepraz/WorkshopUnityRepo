using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingTest : MonoBehaviour
{

    public SkinnedMeshRenderer selfMR;
    MaterialPropertyBlock propertyBlock;
    // Start is called before the first frame update
    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();

        selfMR.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", Color.green);
        selfMR.SetPropertyBlock(propertyBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
