using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float minLookDist = 0.5f;

    private Camera cam;
    private bool isLookEnabled = true;

    private HashSet<int> openUIList = new HashSet<int>();

    public bool IsUIOpen => openUIList.Count > 0;
    public Vector3 MouseWorldPos {  get; private set; } 

    public void Init(Camera cam)
    {
        this.cam = cam;
        
        Managers.Event.AddEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnDisable()
    {
        Managers.Event.RemoveEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnUIChanged(UIChangeEvent data)
    {
        if(data.isOpen) openUIList.Add(data.uiID);
        else openUIList.Remove(data.uiID);

        isLookEnabled = (openUIList.Count == 0);
    }


    private void Update()
    {
        if (cam == null || !isLookEnabled) return;
        
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Ray ray = cam.ScreenPointToRay(Managers.Input.MousePosition);

        if (!groundPlane.Raycast(ray, out float distance)) return;

        Vector3 hitPoint = ray.GetPoint(distance);
        MouseWorldPos = hitPoint;

        Vector3 dir = hitPoint - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude < minLookDist * minLookDist) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
        
    }       

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 10f );
    }
}
