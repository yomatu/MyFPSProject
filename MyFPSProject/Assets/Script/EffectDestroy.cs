using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public float destoryTime;
   
    // Start is called before the first frame update
    void Start()
    {
        //销毁谁,什么时候销毁
        Destroy(gameObject, destoryTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
