using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0,15,-15);

    [SerializeField] private bool enableCursorShift = true;
    [SerializeField] private float nearRadiusVw = 10f;
    [SerializeField] private float nearShift = 0.3f;
    [SerializeField] private float baseFarRadiusVw = 40f;
    [SerializeField] private float lookaheadLerpSpeed = 6f;
    
    [SerializeField] private float farShiftAtZero = 3f;
    [SerializeField] private float farShiftAtOne = 20f; 
    [SerializeField] private bool scaleFarRadius = true;
    [SerializeField] private float farRadiusVwAtOne = 45f;

    [SerializeField] private LayerMask obstructionLayer;
    private ObstructionGroup lastHiddenRoof;

    private float _currentFarShift;
    private float _currentFarRadiusVw;

    private Vector3 _lookahead = Vector3.zero;

    private const float maxVisionRange = 30f;
    private StatBase atkRange;

    private Volume uiBlurVolume;
    private HashSet<int> openUIList = new HashSet<int>();

    public Camera Cam { get; private set; }

    private void Awake()
    {
        Cam = GetComponent<Camera>();

        UIBlurVolumeTag tag = FindObjectOfType<UIBlurVolumeTag>();
        if (tag != null)
        {
            uiBlurVolume = tag.GetComponent<Volume>();
            uiBlurVolume.weight = 0f;
        }
        else
        {
            Logger.Log("UIBlurVolumeTag이 씬에 존재하지 않습니다.");
        }
    }

    private void Start()
    {
        SetViewMultiplier(0f);

        Managers.Event.AddEvent<PlayerSpawnEvent>(OnPlayerSpawn);
        Managers.Event.AddEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnPlayerSpawn(PlayerSpawnEvent data)
    {
        PlayerUnit playerUnit = data.player as PlayerUnit;
        if (playerUnit != null)
        {
            SetTarget(playerUnit.transform);

            atkRange = playerUnit.PlayerStat.GetStatBase(EStatType.AtkRange);

            Managers.Event.RemoveEvent<EquippedEquipmentEvent>(OnEquipmentEquipped);
            Managers.Event.RemoveEvent<UnEquippedEquipmentEvent>(OnEquipmentUnequipped);

            Managers.Event.AddEvent<EquippedEquipmentEvent>(OnEquipmentEquipped);
            Managers.Event.AddEvent<UnEquippedEquipmentEvent>(OnEquipmentUnequipped);
        }
    }

    private void OnUIChanged(UIChangeEvent data)
    {
        if (uiBlurVolume == null) return;
        uiBlurVolume.DOKill();

        if(data.isOpen) openUIList.Add(data.uiID);
        else openUIList.Remove(data.uiID);

        if(openUIList.Count > 0)
        {
            enableCursorShift = false;
            DOTween.To(() => uiBlurVolume.weight, x => uiBlurVolume.weight = x, 1f, 0.3f);
        }
        else
        {
            enableCursorShift = true;
            DOTween.To(() => uiBlurVolume.weight, x => uiBlurVolume.weight = x, 0f, 0.3f);
        }
    }


    private void OnDisable()
    {
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(OnPlayerSpawn);
        Managers.Event.RemoveEvent<EquippedEquipmentEvent>(OnEquipmentEquipped);
        Managers.Event.RemoveEvent<UnEquippedEquipmentEvent>(OnEquipmentUnequipped);
        Managers.Event.RemoveEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnEquipmentEquipped(EquippedEquipmentEvent data)
    {
        UpdateCameraView();
    }

    private void OnEquipmentUnequipped(UnEquippedEquipmentEvent data)
    {
        UpdateCameraView();
    }

    private void UpdateCameraView()
    {
        if (atkRange != null && atkRange.Value != 0)
        {
            SetViewMultiplier(atkRange.Value / maxVisionRange);
        }
        else
        {
            SetViewMultiplier(0f);
        }   
    }

    public void SetTarget(Transform playerTransform) => target = playerTransform;

    public void SetViewMultiplier(float t)
    {
        t = Mathf.Clamp01(t);

        _currentFarShift = Mathf.Lerp(farShiftAtZero, farShiftAtOne, t);

        _currentFarRadiusVw = scaleFarRadius
            ? Mathf.Lerp(baseFarRadiusVw, farRadiusVwAtOne, t)
            : baseFarRadiusVw;
    }


    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 basePos = target.position + offset;

        if (enableCursorShift)
        {
            Vector3 targetLookahead = CalcCursorOffset();
            _lookahead = Vector3.Lerp(_lookahead, targetLookahead, lookaheadLerpSpeed * Time.deltaTime);
        }
        else _lookahead = Vector3.zero;

        transform.position = basePos + _lookahead;

        HandleCameraObstruction();
    }

    private void HandleCameraObstruction()
    {
        Vector3 cameraPos = transform.position;
        Vector3 playerPos = target.position + Vector3.up * 1.8f;

        Vector3 dir = (playerPos - cameraPos).normalized;
        float distance = Vector3.Distance(cameraPos, playerPos);

        if(Physics.Raycast(cameraPos, dir, out RaycastHit hit, distance, obstructionLayer))
        {
            ObstructionGroup roof = hit.collider.GetComponent<ObstructionGroup>();
            if (roof == null) roof = hit.collider.GetComponentInParent<ObstructionGroup>();

            if (roof != null)
            {
                if(lastHiddenRoof != null && lastHiddenRoof != roof)
                {
                    lastHiddenRoof.SetVisibility(true);
                }

                roof.SetVisibility(false);
                lastHiddenRoof = roof;
            }
            else
            {
                if(lastHiddenRoof != null)
                {
                    lastHiddenRoof.SetVisibility(true);
                    lastHiddenRoof = null;
                }
            }
        }
        else
        {
            if(lastHiddenRoof != null)
            {
                lastHiddenRoof.SetVisibility(true);
                lastHiddenRoof = null;
            }
        }
    }
    private Vector3 CalcCursorOffset()
    {
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 offsetVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - screenCenter;

        float vwDist = PixelToVw(offsetVector.magnitude);

        if(vwDist < nearRadiusVw) return Vector3.zero;

        float t = Mathf.Clamp01(Mathf.InverseLerp(nearRadiusVw, _currentFarRadiusVw, vwDist));
        float maxShiftLimit = _currentFarShift;

        Vector2 dir = offsetVector.normalized;

        if(dir.y < 0) maxShiftLimit = _currentFarShift * 1.5f;
        else maxShiftLimit = _currentFarShift * 1.2f;

        float finalShift = Mathf.Lerp(nearShift, maxShiftLimit, t);
        return new Vector3(dir.x, 0, dir.y) * finalShift;
    }

    private float PixelToVw(float pixels)
    {
        float reference = Mathf.Min(Screen.width, Screen.height);

        if (reference == 0) return 0f;
        return (pixels / reference) * 100f;
    }

}
    