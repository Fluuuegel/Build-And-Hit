using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    float mLifeTime = 1.0f;
    float mTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        mTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - mTimer > mLifeTime)
        {
            Destroy(gameObject);
        }    
        
    }
}
