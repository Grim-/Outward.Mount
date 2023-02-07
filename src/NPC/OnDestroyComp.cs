using System;
using UnityEngine;

public class OnDestroyComp : MonoBehaviour
{

    public GameObject MountVisualInstance = null;

    public Action<GameObject> OnDestroyCalled;

    public void OnDestroy()
    {
        if (MountVisualInstance)
        {
            Destroy(MountVisualInstance);
        }

        OnDestroyCalled?.Invoke(gameObject);
    }
}