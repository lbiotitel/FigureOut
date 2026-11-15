using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VisionMeshGenerator : MonoBehaviour
{
    private struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }
    private struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    private IVisionProvider visionProvider;
    private FogRenderConfig config;
    private MeshFilter meshFilter;
    private Mesh viewMesh;
    private MeshRenderer meshRenderer;

    private List<Vector3> viewPoints = new List<Vector3>(256);

    public void Init(IVisionProvider provider, FogRenderConfig renderConfig)
    {
        visionProvider = provider;
        config = renderConfig;

        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

        viewMesh = new Mesh { name = "Vision Mesh" };
        meshFilter.mesh = viewMesh;

        Logger.Log($"[VisionMeshGenerator] Init 호출됨! 프로바이더: {provider.VisionTransform.name}");
    }
    /*
    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        RenderPipelineManager.endCameraRendering += OnEndCamera;
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
        RenderPipelineManager.endCameraRendering -= OnEndCamera;
    }

    private void OnBeginCamera(ScriptableRenderContext ctx, Camera cam)
    {
        if (meshRenderer != null)
            meshRenderer.enabled = cam.CompareTag("FogCamera");
    }

    private void OnEndCamera(ScriptableRenderContext ctx, Camera cam)
    {
        if (cam.CompareTag("FogCamera") && meshRenderer != null)
            meshRenderer.enabled = false;

    }
    */
    private void LateUpdate()
    {
        if (viewMesh == null) return;

        if (visionProvider == null || !visionProvider.IsVisionActive)
        {
            viewMesh.Clear();
            return;
        }

        transform.position = visionProvider.VisionTransform.position;
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(360f * config.MeshResloution);
        float stepAngleSize = 360f / stepCount;

        viewPoints.Clear();

        ViewCastInfo oldViewCast = default;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y
                - 180f
                + stepAngleSize * i;

            float angleDiff = Mathf.DeltaAngle(transform.eulerAngles.y, angle);
            bool inCone = Mathf.Abs(angleDiff) <= visionProvider.ViewAngle * 0.5f;

            float castRadius = inCone ? visionProvider.ViewRadius : visionProvider.CircleRadius;

            ViewCastInfo newViewCast = CastRayWithRadius(angle, castRadius);

            if (i > 0)
            {
                bool hitStateChanged = oldViewCast.hit != newViewCast.hit;

                bool edgeDistExceeded = oldViewCast.hit && newViewCast.hit
                    && Mathf.Abs(oldViewCast.dst - newViewCast.dst) > config.EdgeDstThreshold;

                if (hitStateChanged || edgeDistExceeded)
                {
                    EdgeInfo edge = FindEdgeBinary(oldViewCast, newViewCast, castRadius);
                    if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        BuildMesh(viewPoints);
    }

    private ViewCastInfo CastRayWithRadius(float globalAngle, float radius)
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 dir = DirectionFromAngle(globalAngle);

        if (Physics.Raycast(origin, dir, out RaycastHit hit,
            radius, visionProvider.VisionObstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(
                false,
                origin + dir * radius,
                radius,
                globalAngle
            );
        }
    }

    private EdgeInfo FindEdgeBinary(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float radius)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < config.EdgeResolveIterations; i++)
        {
            float midAngle = (minAngle + maxAngle) * 0.5f;
            ViewCastInfo midCast = CastRayWithRadius(midAngle, radius);

            bool edgeDistExceeded =
                Mathf.Abs(minViewCast.dst - midCast.dst) > config.EdgeDstThreshold;

            if(midCast.hit == minViewCast.hit && !edgeDistExceeded)
            {
                minAngle = midAngle;
                minPoint = midCast.point;
            }
            else
            {
                maxAngle = midAngle;
                maxPoint = midCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private void BuildMesh(List<Vector3> points)
    {
        int vertexCount = points.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        float groundLocalY = 0.1f - transform.position.y;
        vertices[0] = new Vector3(0, groundLocalY, 0);

        for (int i = 0; i < vertexCount - 1; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(points[i]);
            localPos.y = groundLocalY;
            vertices[i + 1] = localPos;

            if (i < vertexCount - 2)
            {
                int ti = i * 3;
                triangles[ti] = 0;
                triangles[ti + 1] = i + 1;
                triangles[ti + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

        viewMesh.RecalculateBounds();
    }

    private static Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(
            Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (visionProvider == null) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, visionProvider.ViewRadius);

        Vector3 left = DirectionFromAngle(
            transform.eulerAngles.y - visionProvider.ViewAngle * 0.5f);
        Vector3 right = DirectionFromAngle(
            transform.eulerAngles.y + visionProvider.ViewAngle * 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, left * visionProvider.ViewRadius);
        Gizmos.DrawRay(transform.position, right * visionProvider.ViewRadius);
    }
#endif
}
