using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

// Monta automaticamente a hierarquia de UI descrita em Guia_UIReacao_Unity.md
// e conecta os campos do componente UIReacao (UIReacao_Aprimorado.cs).
// Usa TMP_DefaultControls diretamente (mesma fábrica dos itens de menu
// GameObject/UI (Canvas)/...) em vez de ExecuteMenuItem, que não funciona
// de forma confiável em modo batch/headless.
public static class UIReacaoSceneBuilder
{
    private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
    private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
    private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
    private const string kKnobPath = "UI/Skin/Knob.psd";
    private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
    private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
    private const string kMaskPath = "UI/Skin/UIMask.psd";

    private const string kScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/TCC/Montar UI Reacao")]
    public static void BuildUI()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != kScenePath)
        {
            scene = EditorSceneManager.OpenScene(kScenePath, OpenSceneMode.Single);
        }

        GameObject canvasExistente = GameObject.Find("Canvas_Reacao");
        if (canvasExistente != null)
            Object.DestroyImmediate(canvasExistente);

        GameObject gerenciadorExistente = GameObject.Find("GerenciadorReacao");
        if (gerenciadorExistente != null)
            Object.DestroyImmediate(gerenciadorExistente);

        var resources = GetStandardResources();

        // ── Canvas + EventSystem ─────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas_Reacao");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        // ── PainelReacao ─────────────────────────────────────────────────
        GameObject painel = CreateUIObject("PainelReacao", canvasGO.transform);
        var painelRect = painel.GetComponent<RectTransform>();
        painelRect.anchorMin = new Vector2(0.5f, 0.5f);
        painelRect.anchorMax = new Vector2(0.5f, 0.5f);
        painelRect.pivot = new Vector2(0.5f, 0.5f);
        painelRect.anchoredPosition = Vector2.zero;
        painelRect.sizeDelta = new Vector2(950, 750);

        var painelImg = painel.AddComponent<Image>();
        painelImg.color = new Color(0.10f, 0.10f, 0.12f, 0.96f);

        var vlg = painel.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(30, 30, 30, 30);
        vlg.spacing = 10;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        CreateText("Titulo", painel.transform, resources, "Simulador de Reações Químicas", 30, FontStyles.Bold, TextAlignmentOptions.Center, 44);

        // O tipo vem primeiro: os reagentes são filtrados pelo tipo escolhido.
        CreateText("LabelTipo", painel.transform, resources, "Tipo de Reação", 16, FontStyles.Normal, TextAlignmentOptions.Left, 26);
        TMP_Dropdown dropTipo = CreateDropdown("DropdownTipo", painel.transform, resources);

        CreateText("LabelReagente1", painel.transform, resources, "Reagente 1", 16, FontStyles.Normal, TextAlignmentOptions.Left, 26);
        TMP_Dropdown dropReagente1 = CreateDropdown("DropdownReagente1", painel.transform, resources);

        Button botaoInverter = CreateButton("BotaoInverter", painel.transform, resources, "↕  Inverter Reagentes", 40);

        CreateText("LabelReagente2", painel.transform, resources, "Reagente 2", 16, FontStyles.Normal, TextAlignmentOptions.Left, 26);
        TMP_Dropdown dropReagente2 = CreateDropdown("DropdownReagente2", painel.transform, resources);

        // Preview
        GameObject previewPanel = CreateUIObject("PreviewPanel", painel.transform);
        var previewLE = previewPanel.AddComponent<LayoutElement>();
        previewLE.minHeight = 70;
        var previewVlg = previewPanel.AddComponent<VerticalLayoutGroup>();
        previewVlg.childControlWidth = true;
        previewVlg.childForceExpandWidth = true;
        previewVlg.spacing = 4;
        CreateText("LabelPreview", previewPanel.transform, resources, "Prévia da Equação", 13, FontStyles.Italic, TextAlignmentOptions.Center, 20);
        TMP_Text txtEquacao = CreateText("TextoEquacao", previewPanel.transform, resources, "Selecione os reagentes para visualizar a equação.", 22, FontStyles.Bold, TextAlignmentOptions.Center, 44);

        // Status
        GameObject statusPanel = CreateUIObject("StatusPanel", painel.transform);
        var statusLE = statusPanel.AddComponent<LayoutElement>();
        statusLE.minHeight = 30;
        TMP_Text txtStatus = CreateText("TextoStatus", statusPanel.transform, resources, "", 16, FontStyles.Normal, TextAlignmentOptions.Center, 30);

        // Botoes
        GameObject botoesRow = CreateUIObject("Botoes", painel.transform);
        var botoesLE = botoesRow.AddComponent<LayoutElement>();
        botoesLE.minHeight = 50;
        var hlg = botoesRow.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 12;
        hlg.childControlWidth = true;
        hlg.childForceExpandWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandHeight = true;

        Button botaoAleatorio = CreateButton("BotaoAleatorio", botoesRow.transform, resources, "Aleatório", 0);
        Button botaoLimpar = CreateButton("BotaoLimpar", botoesRow.transform, resources, "Limpar", 0);
        Button botaoReagir = CreateButton("BotaoReagir", botoesRow.transform, resources, "REAGIR", 0);
        var reagirImg = botaoReagir.GetComponent<Image>();
        if (reagirImg != null) reagirImg.color = new Color(0.20f, 0.55f, 0.28f, 1f);

        // ── PainelHistorico ──────────────────────────────────────────────
        GameObject painelHistorico = CreateUIObject("PainelHistorico", canvasGO.transform);
        var histRect = painelHistorico.GetComponent<RectTransform>();
        histRect.anchorMin = new Vector2(1f, 0.5f);
        histRect.anchorMax = new Vector2(1f, 0.5f);
        histRect.pivot = new Vector2(1f, 0.5f);
        histRect.anchoredPosition = new Vector2(-30, 0);
        histRect.sizeDelta = new Vector2(420, 750);

        var histImg = painelHistorico.AddComponent<Image>();
        histImg.color = new Color(0.08f, 0.08f, 0.10f, 0.9f);

        var histVlg = painelHistorico.AddComponent<VerticalLayoutGroup>();
        histVlg.padding = new RectOffset(20, 20, 20, 20);
        histVlg.spacing = 10;
        histVlg.childControlWidth = true;
        histVlg.childForceExpandWidth = true;
        histVlg.childControlHeight = false;
        histVlg.childForceExpandHeight = false;

        CreateText("TituloHistorico", painelHistorico.transform, resources, "Histórico", 22, FontStyles.Bold, TextAlignmentOptions.Left, 32);
        TMP_Text txtHistorico = CreateText("TextoHistorico", painelHistorico.transform, resources, "Nenhuma reação realizada ainda.", 15, FontStyles.Normal, TextAlignmentOptions.TopLeft, 500);
        Button botaoLimparHistorico = CreateButton("BotaoLimparHistorico", painelHistorico.transform, resources, "Limpar Histórico", 36);
        TMP_Text txtContador = CreateText("TextoContador", painelHistorico.transform, resources, "Reações realizadas: 0", 14, FontStyles.Normal, TextAlignmentOptions.Left, 24);

        // ── Componente UIReacao ──────────────────────────────────────────
        GameObject gerenciador = ObjectFactory.CreateGameObject("GerenciadorReacao");
        var uiReacao = ObjectFactory.AddComponent<UIReacao>(gerenciador);

        MonoScript scriptAsset = null;
        foreach (var candidato in MonoImporter.GetAllRuntimeMonoScripts())
        {
            if (candidato.GetClass() == typeof(UIReacao))
            {
                scriptAsset = candidato;
                break;
            }
        }
        if (scriptAsset != null && scriptAsset.GetClass() == typeof(UIReacao))
        {
            SerializedObject so = new SerializedObject(uiReacao);
            so.FindProperty("m_Script").objectReferenceValue = scriptAsset;
            so.ApplyModifiedProperties();
            Debug.Log("[UIReacaoSceneBuilder] Referência de script corrigida manualmente via SerializedObject.");
        }
        else
        {
            Debug.LogError("[UIReacaoSceneBuilder] Não foi possível resolver o MonoScript de UIReacao. Classe encontrada: " +
                (scriptAsset != null ? scriptAsset.GetClass()?.ToString() : "asset nulo"));
        }

        uiReacao.dropReagente1 = dropReagente1;
        uiReacao.dropReagente2 = dropReagente2;
        uiReacao.dropReacao = dropTipo;
        uiReacao.btnReagir = botaoReagir;
        uiReacao.btnInverter = botaoInverter;
        uiReacao.btnLimpar = botaoLimpar;
        uiReacao.btnAleatorio = botaoAleatorio;
        uiReacao.btnHistoricoLimpar = botaoLimparHistorico;
        uiReacao.txtStatus = txtStatus;
        uiReacao.txtEquacao = txtEquacao;
        uiReacao.txtHistorico = txtHistorico;
        uiReacao.txtContadorReacoes = txtContador;

        EditorUtility.SetDirty(gerenciador);
        EditorUtility.SetDirty(canvasGO);
        EditorSceneManager.MarkSceneDirty(scene);
        bool saved = EditorSceneManager.SaveScene(scene, scene.path);
        if (!saved)
            Debug.LogError("[UIReacaoSceneBuilder] Falha ao salvar a cena em " + scene.path);

        Selection.activeGameObject = gerenciador;
        Debug.Log("[UIReacaoSceneBuilder] UI da reação montada e conectada com sucesso.");
    }

    private static TMP_DefaultControls.Resources GetStandardResources()
    {
        TMP_DefaultControls.Resources r = default;
        r.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
        r.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
        r.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
        r.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
        r.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
        r.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
        r.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
        return r;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static TMP_Text CreateText(string name, Transform parent, TMP_DefaultControls.Resources resources,
        string content, float fontSize, FontStyles style, TextAlignmentOptions align, float minHeight)
    {
        GameObject go = TMP_DefaultControls.CreateText(resources);
        go.name = name;
        go.transform.SetParent(parent, false);

        var txt = go.GetComponent<TextMeshProUGUI>();
        txt.text = content;
        txt.fontSize = fontSize;
        txt.fontStyle = style;
        txt.alignment = align;
        txt.color = Color.white;

        var le = go.AddComponent<LayoutElement>();
        le.minHeight = minHeight;
        le.flexibleWidth = 1;
        return txt;
    }

    private static Button CreateButton(string name, Transform parent, TMP_DefaultControls.Resources resources, string label, float minHeight)
    {
        GameObject go = TMP_DefaultControls.CreateButton(resources);
        go.name = name;
        go.transform.SetParent(parent, false);

        var btnText = go.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null) btnText.text = label;

        var le = go.AddComponent<LayoutElement>();
        if (minHeight > 0) le.minHeight = minHeight;

        return go.GetComponent<Button>();
    }

    private static TMP_Dropdown CreateDropdown(string name, Transform parent, TMP_DefaultControls.Resources resources)
    {
        GameObject go = TMP_DefaultControls.CreateDropdown(resources);
        go.name = name;
        go.transform.SetParent(parent, false);

        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 40;

        return go.GetComponent<TMP_Dropdown>();
    }
}
