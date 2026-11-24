using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    [SerializeField] private CinemachineImpulseSource source;
    private void Awake()
    {
        Instance = this;
        if (source == null)
            source = GetComponent<CinemachineImpulseSource>();
    }
    public void Shake(float force = 0.2f)
    {
        if(source!=null)
            source.GenerateImpulse(force);
    }
}
