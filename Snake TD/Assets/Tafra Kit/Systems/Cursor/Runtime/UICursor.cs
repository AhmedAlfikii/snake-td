using UnityEngine;

public class UICursor : MonoBehaviour
{
    void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
