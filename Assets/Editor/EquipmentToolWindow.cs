using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class EquipmentToolWindow : EditorWindow
{
    private const float PreviewWidth = 600f;
    private const float MarkerSize = 5f;

    private GameObject previewInstance;
    private PreviewRenderUtility previewUtility;
    private GameObject currentModelPrefab;

    private Vector2 scroll;
    private Vector2 previewDir = new Vector2(120f, -20f);
    private float zoom = 1.0f;

    private SerializedObject currentSerializeObject;
    private EquipmentData currentEquipmentData;

    [MenuItem("Tools/Equipment Tool")]
    public static void ShowWindow()
    {
        GetWindow<EquipmentToolWindow>("Equipment Tool");
    }

    private void OnEnable()
    {
        previewUtility = new PreviewRenderUtility();
        previewUtility.cameraFieldOfView = 30f;

        previewUtility.lights[0].intensity = 1.2f;
        previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);

        previewUtility.lights[1].intensity = 1.0f;
        previewUtility.lights[1].transform.rotation = Quaternion.Euler(340f, 218f, 177f);
    }

    private void OnDisable()
    {
        CleanupPreviewInstance();

        if (previewUtility != null)
        {
            previewUtility.Cleanup();
            previewUtility = null;
        }
    }

    private void OnGUI()
    {
        DrawTopToolbar();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();

        DrawPreviewPanel();
        DrawDataPanel();

        EditorGUILayout.EndHorizontal();
    }

    #region Top Toolbar

    private void DrawTopToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        DrawSelectField();

        GUILayout.Space(8);

        if (GUILayout.Button("원거리 무기 생성", EditorStyles.toolbarButton, GUILayout.Width(120f)))
        {
            CreateEquipmentData<RangedWeaponData>();
        }

        if (GUILayout.Button("근접 무기 생성", EditorStyles.toolbarButton, GUILayout.Width(120f)))
        {
            CreateEquipmentData<MeleeWeaponData>();
        }

        if (GUILayout.Button("방어구 생성", EditorStyles.toolbarButton, GUILayout.Width(120f)))
        {
            CreateEquipmentData<ArmorData>();
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSelectField()
    {
        EditorGUI.BeginChangeCheck();

        EquipmentData newData = (EquipmentData)EditorGUILayout.ObjectField(
            currentEquipmentData,
            typeof(EquipmentData),
            false,
            GUILayout.Width(250f)
        );

        if (EditorGUI.EndChangeCheck())
        {
            SetCurrentEquipmentData(newData);
        }
    }

    private void SetCurrentEquipmentData(EquipmentData equipmentData)
    {
        currentEquipmentData = equipmentData;
        currentSerializeObject = currentEquipmentData != null ? new SerializedObject(currentEquipmentData) : null;

        if (currentSerializeObject != null)
        {
            SerializedProperty prefabProp = currentSerializeObject.FindProperty("modelPrefab");
            GameObject prefab = prefabProp != null ? prefabProp.objectReferenceValue as GameObject : null;
            SetPreviewPrefab(prefab);
        }
        else
        {
            SetPreviewPrefab(null);
        }

        Repaint();
    }

    private void CreateEquipmentData<T>() where T : EquipmentData
    {
        T newData = ScriptableObject.CreateInstance<T>();

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Equipment Data",
            typeof(T).Name,
            "asset",
            "저장할 위치 선택"
        );

        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(newData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = newData;
        SetCurrentEquipmentData(newData);
    }


    #endregion

    #region Data Panel

    private void DrawDataPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

        if (currentSerializeObject == null)
        {
            EditorGUILayout.HelpBox("아이템 데이터를 생성하거나 선택하세요.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        currentSerializeObject.Update();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawItemCommonData();
        DrawEquipmentCommonData();

        if (currentEquipmentData is WeaponData)
        {
            DrawWeaponCommonData();
            DrawWeaponData();
        }
        else if (currentEquipmentData is ArmorData)
        {
            DrawArmorData();
        }

        GUILayout.Space(12f);

        if (GUILayout.Button("저장", GUILayout.Height(28f)))
        {
            SaveAsset();
        }

        EditorGUILayout.EndScrollView();

        currentSerializeObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
    }

    private void SaveAsset()
    {
        currentSerializeObject.ApplyModifiedProperties();

        string itemId = currentEquipmentData.ItemID;
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("itemId 없음");
            return;
        }

        EnsureFolderExists();
        string path = $"{EditorDefines.WEAPON_SAVE_PATH}{itemId}.asset";

        var existingAsset = AssetDatabase.LoadAssetAtPath<EquipmentData>(path);

        if (existingAsset != null)
        {
            EditorUtility.CopySerialized(currentEquipmentData, existingAsset);
            EditorUtility.SetDirty(existingAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"에셋 업데이트/저장 완료: {path}");
    }

    private void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Datas"))
            AssetDatabase.CreateFolder("Assets", "Datas");

        if (!AssetDatabase.IsValidFolder("Assets/Datas/Weapons"))
            AssetDatabase.CreateFolder("Assets/Datas", "Weapons");
    }

    private void DrawItemCommonData()
    {
        GUILayout.Label("아이템 공통 속성", EditorStyles.boldLabel);

        DrawProperty("itemId");
        DrawProperty("itemName");
        DrawProperty("itemDescription");
        DrawProperty("itemIcon");
        DrawProperty("itemType");
        DrawProperty("rarity");
        DrawProperty("stackType");
        DrawProperty("weight");
        DrawProperty("price");

        SerializedProperty prefabProp = currentSerializeObject.FindProperty("modelPrefab");
        if (prefabProp != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(prefabProp);

            if (EditorGUI.EndChangeCheck())
            {
                currentSerializeObject.ApplyModifiedProperties();
                GameObject prefab = prefabProp.objectReferenceValue as GameObject;
                SetPreviewPrefab(prefab);
            }
        }


        GUILayout.Space(10f);
    }

    private void DrawEquipmentCommonData()
    {
        GUILayout.Label("장비 공통 속성", EditorStyles.boldLabel);


        DrawProperty("equipmentType");
        GUILayout.Label("장비 스탯", EditorStyles.boldLabel);
        DrawProperty("equipmentStats");
        GUILayout.Label("진화 레시피 ID", EditorStyles.boldLabel);
        DrawProperty("revolutionRecipeID");
        GUILayout.Label("장비 모델 스폰 위치 보정 값", EditorStyles.boldLabel);
        DrawProperty("grabPos");
    }


    private void DrawWeaponCommonData()
    {
        GUILayout.Label("무기 공통 속성", EditorStyles.boldLabel);
        DrawProperty("upgradeWeaponID");
        DrawProperty("soundSFX");
        DrawProperty("firePos");
    }


    private void DrawWeaponData()
    {
        if (typeof(RangedWeaponData).IsAssignableFrom(currentEquipmentData))
        {
            DrawProperty("ammoType");
            DrawProperty("reloadTime");
            DrawProperty("magazineSize");
            DrawProperty("recoil");
            DrawProperty("soundSize");
            DrawProperty("projectileData");
        }

        if (typeof(MeleeWeaponData).IsAssignableFrom(currentEquipmentData))
        {


        }
    }

    private void DrawArmorData()
    {

    }

    private void DrawProperty(string propertyName)
    {
        SerializedProperty prop = currentSerializeObject.FindProperty(propertyName);
        if (prop != null)
        {
            EditorGUILayout.PropertyField(prop);
        }
    }

    #endregion

    #region Preview

    private void CleanupPreviewInstance()
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
    }

    private void SetPreviewPrefab(GameObject newPrefab)
    {
        if (currentModelPrefab == newPrefab)
            return;

        currentModelPrefab = newPrefab;

        CleanupPreviewInstance();

        if (currentModelPrefab != null)
        {
            previewInstance = PrefabUtility.InstantiatePrefab(currentModelPrefab) as GameObject;
            if (previewInstance != null)
            {
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        Repaint();
    }

    private void DrawPreviewPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(PreviewWidth));

        EditorGUILayout.LabelField("모델 프리뷰", EditorStyles.boldLabel);
        GUILayout.Space(4f);

        Rect previewRect = GUILayoutUtility.GetRect(
            10f,
            1000f,
            300f,
            500f,
            GUILayout.Width(PreviewWidth),
            GUILayout.ExpandHeight(true)
        );

        DrawPreview(previewRect);
        HandleInput(previewRect);

        EditorGUILayout.EndVertical();
    }

    private void HandleInput(Rect rect)
    {
        Event e = Event.current;

        if (!rect.Contains(e.mousePosition))
            return;

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            e.Use();
            return;
        }

        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            previewDir -= e.delta * 0.5f;
            Repaint();
            e.Use();
            return;
        }

        if (e.type == EventType.ScrollWheel)
        {
            zoom += e.delta.y * 0.05f;
            zoom = Mathf.Clamp(zoom, 0.2f, 5.0f);
            Repaint();
            e.Use();
            return;
        }
    }



    private void DrawPreview(Rect rect)
    {
        if (previewUtility == null)
            return;

        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));

        if (previewInstance == null)
        {
            GUI.Label(rect, "무기 모델을 넣으세요", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        Bounds bounds = CalculateBounds(previewInstance);

        float size = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        if (size < 0.1f)
            size = 0.1f;

        Quaternion rot = Quaternion.Euler(-previewDir.y, -previewDir.x, 0f);
        Vector3 target = bounds.center;
        Vector3 camPos = target + rot * (Vector3.back * size * 4f * zoom);

        previewUtility.BeginPreview(rect, GUIStyle.none);

        Camera cam = previewUtility.camera;
        cam.transform.position = camPos;
        cam.transform.rotation = rot;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 1000f;
        cam.clearFlags = CameraClearFlags.Color;
        cam.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
        cam.transform.LookAt(target);

        DrawAllMeshes(previewInstance);

        cam.Render();

        Texture resultRender = previewUtility.EndPreview();
        GUI.DrawTexture(rect, resultRender, ScaleMode.StretchToFill, false);

        DrawAxisGizmo(rect, cam);
        DrawSocketMarkers(rect, cam);
    }


    private void DrawSocketMarkers(Rect rect, Camera cam)
    {
        if (currentEquipmentData == null || previewInstance == null)
            return;

        Vector3 grabWorld = previewInstance.transform.TransformPoint(currentEquipmentData.GrabPos);
        Vector3 fireWorld = Vector3.zero;
        if (currentEquipmentData.EquipmentType == EEquipmentType.Weapon)
        {
            WeaponData weaponData = currentEquipmentData as WeaponData;
            fireWorld = previewInstance.transform.TransformPoint(weaponData.FirePos);
            DrawWorldMarker(grabWorld, "Grab", cam, rect, Color.green);
            DrawWorldMarker(fireWorld, "Fire", cam, rect, Color.red);
        }
        else if (currentEquipmentData.EquipmentType == EEquipmentType.Helmet ||
            currentEquipmentData.EquipmentType == EEquipmentType.Chestplate)
        {
            DrawWorldMarker(grabWorld, "Grab", cam, rect, Color.green);
        }
    }

    private void DrawWorldMarker(Vector3 worldPos, string label, Camera cam, Rect previewRect, Color color)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0f)
            return;

        Vector2 guiPos = new Vector2(
            previewRect.x + screenPos.x,
            previewRect.y + (previewRect.height - screenPos.y)
        );

        Handles.BeginGUI();

        Color prevColor = Handles.color;
        Handles.color = color;
        Handles.DrawSolidDisc(guiPos, Vector3.forward, MarkerSize);
        Handles.color = prevColor;

        Handles.EndGUI();

        GUIStyle style = new GUIStyle(EditorStyles.whiteMiniLabel);
        style.normal.textColor = color;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(
            new Rect(guiPos.x + 8f, guiPos.y - 10f, 60f, 20f),
            label,
            style
        );
    }

    private void DrawAxisGizmo(Rect rect, Camera cam)
    {
        Rect gizmoRect = new Rect(rect.xMax - 82f, rect.y + 10f, 72f, 72f);

        GUI.Box(gizmoRect, GUIContent.none);

        GUI.BeginGroup(gizmoRect);
        Handles.BeginGUI();

        Vector2 center = new Vector2(36f, 36f);

        DrawAxis(cam.transform.right, center, Color.red, "X");
        DrawAxis(cam.transform.up, center, Color.green, "Y");
        DrawAxis(cam.transform.forward, center, Color.blue, "Z");

        Handles.EndGUI();
        GUI.EndGroup();
    }

    private void DrawAxis(Vector3 dir, Vector2 center, Color color, string label)
    {
        Handles.color = color;

        Vector2 projected = new Vector2(dir.x, -dir.y) * 22f;
        Handles.DrawLine(center, center + projected);

        GUIStyle style = new GUIStyle(EditorStyles.whiteMiniLabel);
        style.normal.textColor = color;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(center + projected + new Vector2(2f, -8f), new Vector2(18f, 16f)), label, style);
    }

    private void DrawAllMeshes(GameObject root)
    {
        MeshFilter[] meshFilters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (MeshFilter mf in meshFilters)
        {
            MeshRenderer renderer = mf.GetComponent<MeshRenderer>();
            if (mf.sharedMesh == null || renderer == null)
                continue;

            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null)
                    continue;

                previewUtility.DrawMesh(
                    mf.sharedMesh,
                    mf.transform.localToWorldMatrix,
                    materials[i],
                    i
                );
            }
        }

        SkinnedMeshRenderer[] skinnedMeshes = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (SkinnedMeshRenderer smr in skinnedMeshes)
        {
            if (smr.sharedMesh == null)
                continue;

            Material[] materials = smr.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null)
                    continue;

                previewUtility.DrawMesh(
                    smr.sharedMesh,
                    smr.transform.localToWorldMatrix,
                    materials[i],
                    i
                );
            }
        }
    }

    private Bounds CalculateBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);

        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }

    #endregion
}