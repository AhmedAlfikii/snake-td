using UnityEngine;

[ExecuteInEditMode]
public class Billboard : MonoBehaviour
{
    [SerializeField] private bool workInEditMode;

    private Transform cam;

    void Start()
    {
        //cam = LevelManager.MainCamera.transform;
        cam = Camera.main.transform;      //Optimize this in actual game project.
        
        if (workInEditMode)
            transform.forward = cam.forward;
    }

    void LateUpdate()
    {
        if(Application.isPlaying)
        {
            if(cam)
                transform.forward = cam.forward;
        }

#if UNITY_EDITOR
        if (workInEditMode)
        {
            if (cam == null)
                cam = Camera.main.transform;

            if (cam)
                transform.forward = cam.forward;
        }
#endif
    }
}