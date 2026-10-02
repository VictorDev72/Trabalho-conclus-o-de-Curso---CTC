using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using TMPro;
using BalanciadorQuimico;

// Cria o prefab de átomo genérico e a cena "Reacao3D" que exibe as moléculas
// balanceadas (via ReacaoTransferData) usando GerenciadorVisual3D + Atomo3D.
public static class Reacao3DSceneBuilder
{
    private const string kPrefabFolder = "Assets/Prefabs";
    private const string kPrefabPath = "Assets/Prefabs/AtomoBase_Prefab.prefab";
    private const string kScenePath = "Assets/Scenes/Reacao3D.unity";
    private const string kSampleScenePath = "Assets/Scenes/SampleScene.unity";



    [MenuItem("Tools/TCC/Montar Cena 3D da Reacao")]
    public static void BuildAll()
    {
        GameObject prefab = CreateOrUpdateAtomoPrefab();
        
        CreateReacao3DScene(prefab);
        RegisterScenesInBuildSettings();
        Debug.Log("[Reacao3DSceneBuilder] Prefab e cena 3D montados com sucesso.");
    }

    private static GameObject CreateOrUpdateAtomoPrefab()
    {
        if (!AssetDatabase.IsValidFolder(kPrefabFolder))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "AtomoBase_Prefab";
        Object.DestroyImmediate(sphere.GetComponent<Collider>());
        ObjectFactory.AddComponent<Atomo3D>(sphere);

        bool isNew = !System.IO.File.Exists(kPrefabPath);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(sphere, kPrefabPath);
        Object.DestroyImmediate(sphere);

        Debug.Log(isNew
            ? "[Reacao3DSceneBuilder] Prefab criado em " + kPrefabPath
            : "[Reacao3DSceneBuilder] Prefab atualizado em " + kPrefabPath);
        return prefab;
    }

    private static void CreateReacao3DScene(GameObject atomoPrefab)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Câmera
        GameObject camGO = ObjectFactory.CreateGameObject("Main Camera", typeof(Camera));
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(4f, 6f, -20f);
        camGO.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
        var cam = camGO.GetComponent<Camera>();
        cam.fieldOfView = 55f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 200f;
        camGO.AddComponent<AudioListener>();
        var cameraMovel = camGO.AddComponent<CameraMovel>();

        cameraMovel.velocidade = 5f;
        cameraMovel.velocidadeTurbo = 10f;
        cameraMovel.sensibilidadeMouse = 2f;
        cameraMovel.velocidadeZoom = 5f;

        // Luz direcional
        GameObject lightGO = ObjectFactory.CreateGameObject("Directional Light", typeof(Light));
        var light = lightGO.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Gerenciador visual
        GameObject gerenciadorGO = ObjectFactory.CreateGameObject("GerenciadorVisual3D");
        var gerenciador = ObjectFactory.AddComponent<GerenciadorVisual3D>(gerenciadorGO);
        gerenciador.atomoPrefab = atomoPrefab;

        // Canvas com título + botão Voltar
        GameObject canvasGO = new GameObject("Canvas_3D");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<InputSystemUIInputModule>();

        var resources = GetStandardResources();

        GameObject tituloGO = TMP_DefaultControls.CreateText(resources);
        tituloGO.name = "TituloReacao3D";
        tituloGO.transform.SetParent(canvasGO.transform, false);
        var tituloRect = tituloGO.GetComponent<RectTransform>();
        tituloRect.anchorMin = new Vector2(0.5f, 1f);
        tituloRect.anchorMax = new Vector2(0.5f, 1f);
        tituloRect.pivot = new Vector2(0.5f, 1f);
        tituloRect.anchoredPosition = new Vector2(0, -30);
        tituloRect.sizeDelta = new Vector2(900, 50);
        var tituloTxt = tituloGO.GetComponent<TextMeshProUGUI>();
        tituloTxt.text = "Visualização 3D da Reação — Reagentes à esquerda, Produtos à direita";
        tituloTxt.fontSize = 24;
        tituloTxt.alignment = TextAlignmentOptions.Center;
        tituloTxt.color = Color.white;
        ObjectFactory.AddComponent<TituloReacao3D>(tituloGO);

        GameObject btnGO = TMP_DefaultControls.CreateButton(resources);
        btnGO.name = "BotaoVoltar";
        btnGO.transform.SetParent(canvasGO.transform, false);
        var btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0f, 0f);
        btnRect.anchorMax = new Vector2(0f, 0f);
        btnRect.pivot = new Vector2(0f, 0f);
        btnRect.anchoredPosition = new Vector2(30, 30);
        btnRect.sizeDelta = new Vector2(160, 44);
        var btnTxt = btnGO.GetComponentInChildren<TextMeshProUGUI>();
        if (btnTxt != null) btnTxt.text = "◀ Voltar";
        var button = btnGO.GetComponent<Button>();

        GameObject voltarGO = ObjectFactory.CreateGameObject("VoltarParaMenu");
        var voltar = ObjectFactory.AddComponent<VoltarParaMenu>(voltarGO);
        voltar.cena = "SampleScene";

        UnityEventTools.AddPersistentListener(button.onClick, voltar.Voltar);

        EditorUtility.SetDirty(gerenciadorGO);
        bool saved = EditorSceneManager.SaveScene(scene, kScenePath);
        if (!saved)
            Debug.LogError("[Reacao3DSceneBuilder] Falha ao salvar a cena em " + kScenePath);
    }

    private static void RegisterScenesInBuildSettings()
    {
        var lista = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        bool temSample = lista.Exists(s => s.path == kSampleScenePath);
        bool temReacao3D = lista.Exists(s => s.path == kScenePath);

        if (!temSample)
            lista.Add(new EditorBuildSettingsScene(kSampleScenePath, true));
        if (!temReacao3D)
            lista.Add(new EditorBuildSettingsScene(kScenePath, true));

        EditorBuildSettings.scenes = lista.ToArray();

        // Reabre a SampleScene como cena ativa ao final.
        EditorSceneManager.OpenScene(kSampleScenePath, OpenSceneMode.Single);
    }

    private static TMP_DefaultControls.Resources GetStandardResources()
    {
        TMP_DefaultControls.Resources r = default;
        r.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        r.background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        r.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        r.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        r.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        r.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        r.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
        return r;
    }
}
