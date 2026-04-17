using System.Collections;
using UnityEngine;

public class SceneCamera : MonoBehaviour
{

    public static SceneCamera Inst;
    public static Vector3 cursorPos;
    [HideInInspector] public Camera cam;

    void Awake()
    {
        Inst = this;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cursorPos = WorldPositionFromMouse();
        
    }

    public Vector3 WorldPositionFromMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane;
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public void Shake(float strength, float duration = .05f)
    {
        StartCoroutine(DoShake(strength, duration));
    }

    IEnumerator DoShake(float strength, float duration)
    {
        Vector3 startPos = transform.position;
        float timer = 0;

        while(timer < duration)
        {
            
            timer += Time.deltaTime;
            transform.position = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPos;
    }
}
