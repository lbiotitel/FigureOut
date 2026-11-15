using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager Instance { get; private set; }

    [SerializeField] private FogRenderConfig config;
    [SerializeField] private Camera fogCamera;
    [SerializeField] private Renderer fogPlaneRenderer;

    private List<IVisionProvider> visionProviders = new List<IVisionProvider>();

    private RenderTexture fogMaskRT;

    private static int FogMaskID = Shader.PropertyToID("_FogMask");
    private static int FogColorID = Shader.PropertyToID("_FogColor");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitRederTexture();

        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawn);

        ReleaseRenderTexture();
    }

    public void RegisterVisionProvider(IVisionProvider provider)
    {
        if (provider == null || visionProviders.Contains(provider))
            return;

        visionProviders.Add(provider);
        Logger.Log($"[FogOfWarManager] VisionProvider 등록: {provider.VisionTransform.name}");
    }

    public void UnregisterVisionProvider(IVisionProvider provider)
    {
        visionProviders.Remove(provider);
    }

    public IReadOnlyList<IVisionProvider> GetVisionProviders()
        => visionProviders.AsReadOnly();

    private void InitRederTexture()
    {
        ReleaseRenderTexture();

        fogMaskRT = new RenderTexture(
            config.Resolution,
            config.Resolution,
            depth: 0,
            format: RenderTextureFormat.ARGB32
        );
        fogMaskRT.filterMode = config.FilterMode;
        fogMaskRT.wrapMode = TextureWrapMode.Clamp;
        fogMaskRT.antiAliasing = 8;
        fogMaskRT.Create();

        if (fogCamera != null)
        {
            fogCamera.targetTexture = fogMaskRT;
            fogCamera.orthographic = true;
            fogCamera.orthographicSize = config.OrthographicSize;
            fogCamera.cullingMask = config.VisionLayer;
            Logger.Log("[FogOfWarManager] FogCamera RT 연결 완료");
        }
        else
        {
            Logger.LogError("[FogOfWarManager] FogCamera가 할당되지 않았습니다.");
        }

        if (fogPlaneRenderer != null)
        {
            //var propBlock = new MaterialPropertyBlock();
            //fogPlaneRenderer.GetPropertyBlock(propBlock);
            //propBlock.SetTexture(FogMaskID, fogMaskRT);
            //propBlock.SetColor(FogColorID, config.FogColor);
            //fogPlaneRenderer.SetPropertyBlock(propBlock);

            //fogPlaneRenderer.material.SetTexture(FogMaskID, fogMaskRT);
            //fogPlaneRenderer.material.SetColor(FogColorID, config.FogColor);

            //Logger.Log("[FogOfWarManager] FogPlane에 FogMask 연결 완료");

            Shader.SetGlobalTexture("_GlobalFogMask", fogMaskRT);
            Shader.SetGlobalVector("_FogCamParams", new Vector4(fogCamera.transform.position.x, fogCamera.transform.position.z, config.OrthographicSize, 0));
            fogPlaneRenderer.material.SetColor("_FogColor", config.FogColor);

            //Vector3 camPos = fogCamera.transform.position;
            //fogPlaneRenderer.transform.position = new Vector3(camPos.x, fogPlaneRenderer.transform.position.y, camPos.z);

            float planeScale = (config.OrthographicSize * 2f) / 10f;
            fogPlaneRenderer.transform.localScale = new Vector3(planeScale, 1f, planeScale);

            Logger.Log("[FogOfWarManager] 글로벌 텍스처 매핑 완료");
        }
        else
        {
            Logger.LogError("[FogOfWarManager] FogPlane Renderer가 할당되지 않았습니다.");
        }
    }

    private void ReleaseRenderTexture()
    {
        if (fogMaskRT == null)  return;

        if (fogCamera != null)
            fogCamera.targetTexture = null;

        fogMaskRT.Release();
        Destroy(fogMaskRT);
        fogMaskRT = null;
    }

    private void HandlePlayerSpawn(PlayerSpawnEvent data)
    {
        PlayerUnit player = data.player as PlayerUnit;
        if (player == null) return;

        if(player is IVisionProvider visionProvider)
        {
            RegisterVisionProvider(visionProvider);

            GameObject visionMeshObj = new GameObject("VisionMesh");
            visionMeshObj.transform.SetParent(player.transform);
            visionMeshObj.transform.localPosition = Vector3.zero;

            visionMeshObj.layer = LayerMask.NameToLayer("VisionLayer");

            // 3. MeshFilter와 MeshRenderer 수동 추가
            visionMeshObj.AddComponent<MeshFilter>();
            MeshRenderer renderer = visionMeshObj.AddComponent<MeshRenderer>();

            // 4. VisionMaskWriter 셰이더를 사용하는 머티리얼 할당
            // (주의: 빌드 시 셰이더가 포함되도록 Graphics 설정의 Always Included Shaders에 Hidden/VisionMaskWriter를 추가해야 합니다)
            Shader maskShader = Shader.Find("Hidden/VisionMaskWriter");
            if (maskShader != null)
            {
                renderer.material = new Material(maskShader);
            }
            else
            {
                Logger.LogError("VisionMaskWriter 셰이더를 찾을 수 없습니다.");
            }

            VisionMeshGenerator meshGen = visionMeshObj.AddComponent<VisionMeshGenerator>();
            meshGen.Init(visionProvider, config);

            VisionDetector detector = player.gameObject.AddComponent<VisionDetector>();
            detector.Init(visionProvider);
        }
    }
}
