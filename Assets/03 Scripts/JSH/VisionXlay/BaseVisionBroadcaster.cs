using UnityEngine;

public abstract class BaseVisionBroadcaster : MonoBehaviour
{
    private int targetPosId;
    private int radiusId;

    private float currentVisionRadius = 0f;
    private bool isInit = false;

    [SerializeField] private float visualRadiusPadding = 0.5f;

    public virtual void Init(string posShaderName, string radiusShaderName, float radius)
    {
        targetPosId = Shader.PropertyToID(posShaderName);
        radiusId = Shader.PropertyToID(radiusShaderName);

        Shader.SetGlobalVector(targetPosId, new Vector3(9999f, 0f, 9999f));
        Shader.SetGlobalFloat(radiusId, 0f);

        currentVisionRadius = radius;
        isInit = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isInit)
        {
            Shader.SetGlobalVector(targetPosId, GetTargetPosition());
            Shader.SetGlobalFloat(radiusId, currentVisionRadius + visualRadiusPadding);
        }
    }

    protected virtual void OnDisable()
    {
        if (isInit) Shader.SetGlobalFloat(radiusId, 0f);
        isInit = false;
    }

    protected abstract Vector3 GetTargetPosition();
}
