using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace VisualProgramming
{
    /// <summary>
    /// Runtime-generated, responsive visual programming editor.
    /// Desktop: palette / workspace / object inspector.
    /// Mobile: palette above workspace.
    /// </summary>
    public class VisualProgramEditor : MonoBehaviour
    {
        [Header("Runtime")]
        [SerializeField] private VisualProgramRunner runner;

        [Header("Theme")]
        [SerializeField] private Color backgroundColor = new Color32(15, 20, 30, 255);
        [SerializeField] private Color panelColor = new Color32(27, 37, 52, 255);
        [SerializeField] private Color panelInnerColor = new Color32(18, 27, 40, 255);
        [SerializeField] private Color blockColor = new Color32(43, 59, 79, 255);
        [SerializeField] private Color accentColor = new Color32(55, 143, 231, 255);
        [SerializeField] private Color dangerColor = new Color32(183, 76, 80, 255);

        private VisualProgramData program = new VisualProgramData();

        private Canvas canvas;
        private Font uiFont;
        private InputField targetField;
        private RectTransform palettePanel;
        private RectTransform workspacePanel;
        private RectTransform inspectorPanel;
        private RectTransform blockList;
        private Text statusText;
        private Text inspectorText;
        private GridLayoutGroup paletteGrid;
        private bool mobileLayout;
        private int lastScreenWidth;
        private int lastScreenHeight;

        private const float OuterMargin = 12f;
        private const float HeaderHeight = 64f;
        private const float TargetHeight = 48f;
        private const float FooterHeight = 34f;

        private void Awake()
        {
            if (runner == null)
                runner = GetComponent<VisualProgramRunner>();
            if (runner == null)
                runner = gameObject.AddComponent<VisualProgramRunner>();

            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            program.blocks ??= new List<VisualBlockData>();
            BuildUI();
            RefreshBlocks();

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }

        private void Update()
        {
            if (Screen.width == lastScreenWidth && Screen.height == lastScreenHeight)
                return;

            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            ApplyResponsiveLayout();
        }

        private void BuildUI()
        {
            canvas = CreateCanvas();

            Image background = canvas.gameObject.AddComponent<Image>();
            background.color = backgroundColor;

            CreateHeader(canvas.transform);
            CreateTargetBar(canvas.transform);
            CreateFooter(canvas.transform);
            CreateMainPanels(canvas.transform);

            Canvas.ForceUpdateCanvases();
            ApplyResponsiveLayout();
        }

        private Canvas CreateCanvas()
        {
            GameObject go = new GameObject(
                "VisualProgrammingCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            Canvas result = go.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.sortingOrder = 100;

            CanvasScaler scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = IsMobileScreen()
                ? new Vector2(900f, 1600f)
                : new Vector2(1440f, 900f);
            scaler.matchWidthOrHeight = IsMobileScreen() ? 0f : 0.5f;

            return result;
        }

        private bool IsMobileScreen()
        {
            return Screen.width < 900 || Screen.height > Screen.width;
        }

        private void CreateHeader(Transform parent)
        {
            RectTransform header = CreatePanel(
                "Header", parent, panelColor,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(OuterMargin, -HeaderHeight),
                new Vector2(-OuterMargin, -OuterMargin));

            AddText("UNITY BLOCK LAB", header, 20, Color.white,
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(12, 0), new Vector2(190, 0),
                TextAnchor.MiddleLeft, FontStyle.Bold);

            float x = 205f;
            AddHeaderButton("▶  Run", header, accentColor, RunProgram, x, 100);
            x += 108f;
            AddHeaderButton("■  Stop", header, dangerColor, StopProgram, x, 100);
            x += 108f;
            AddHeaderButton("Save", header, blockColor, SaveProgram, x, 84);
            x += 92f;
            AddHeaderButton("Load", header, blockColor, LoadProgram, x, 84);
        }

        private void CreateTargetBar(Transform parent)
        {
            RectTransform bar = CreatePanel(
                "TargetBar", parent, panelColor,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(OuterMargin, -(OuterMargin + HeaderHeight + 10f + TargetHeight)),
                new Vector2(-OuterMargin, -(OuterMargin + HeaderHeight + 10f)));

            AddText("TARGET OBJECT", bar, 12, new Color32(166, 190, 218, 255),
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(12, 0), new Vector2(128, 0),
                TextAnchor.MiddleLeft, FontStyle.Bold);

            RectTransform inputRect = CreateRect(
                "TargetNameInput", bar,
                new Vector2(0, 0.5f), new Vector2(0, 0.5f),
                new Vector2(132, 0), new Vector2(330, 34));

            targetField = CreateInputField(inputRect, "Cube", 14);
            targetField.onValueChanged.AddListener(_ => UpdateInspector());

            AddText("Scene内のGameObject名を指定", bar, 11,
                new Color32(143, 163, 188, 255),
                new Vector2(0, 0), new Vector2(1, 1),
                new Vector2(342, 0), new Vector2(-12, 0),
                TextAnchor.MiddleLeft);
        }

        private void CreateFooter(Transform parent)
        {
            RectTransform footer = CreatePanel(
                "Footer", parent, panelColor,
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(OuterMargin, OuterMargin),
                new Vector2(-OuterMargin, OuterMargin + FooterHeight));

            statusText = AddText(
                "Ready  •  ブロックを追加して Run を押してください",
                footer, 11, new Color32(161, 185, 212, 255),
                Vector2.zero, Vector2.one,
                new Vector2(10, 0), new Vector2(-10, 0),
                TextAnchor.MiddleLeft);
        }

        private void CreateMainPanels(Transform parent)
        {
            RectTransform main = CreateRect(
                "MainArea", parent,
                Vector2.zero, Vector2.one,
                new Vector2(OuterMargin, OuterMargin + FooterHeight + 10f),
                new Vector2(-OuterMargin, -(OuterMargin + HeaderHeight + 10f + TargetHeight + 10f)));

            palettePanel = CreatePanel(
                "BlockPalette", main, panelColor,
                new Vector2(0, 0), new Vector2(0.22f, 1),
                Vector2.zero, Vector2.zero);

            workspacePanel = CreatePanel(
                "Workspace", main, panelColor,
                new Vector2(0.23f, 0), new Vector2(0.77f, 1),
                Vector2.zero, Vector2.zero);

            inspectorPanel = CreatePanel(
                "ObjectInspector", main, panelColor,
                new Vector2(0.78f, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero);

            BuildPaletteContents(palettePanel);
            BuildWorkspaceContents(workspacePanel);
            BuildInspectorContents(inspectorPanel);
        }

        private void BuildPaletteContents(RectTransform panel)
        {
            AddText("BLOCKS", panel, 14, Color.white,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(12, -38), new Vector2(-12, -8),
                TextAnchor.MiddleLeft, FontStyle.Bold);

            RectTransform gridRect = CreateRect(
                "PaletteGrid", panel,
                Vector2.zero, Vector2.one,
                new Vector2(10, 10), new Vector2(-10, -46));

            paletteGrid = gridRect.gameObject.AddComponent<GridLayoutGroup>();
            paletteGrid.padding = new RectOffset(0, 0, 0, 0);
            paletteGrid.spacing = new Vector2(6, 6);
            paletteGrid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            paletteGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
            paletteGrid.childAlignment = TextAnchor.UpperLeft;
            paletteGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            AddPaletteButton("移動", VisualBlockType.Move, gridRect);
            AddPaletteButton("回転", VisualBlockType.Rotate, gridRect);
            AddPaletteButton("位置指定", VisualBlockType.SetPosition, gridRect);
            AddPaletteButton("表示切替", VisualBlockType.SetVisible, gridRect);
            AddPaletteButton("サイズ", VisualBlockType.SetScale, gridRect);
            AddPaletteButton("待機", VisualBlockType.Wait, gridRect);
            AddPaletteButton("ログ", VisualBlockType.DebugLog, gridRect);
        }

        private void BuildWorkspaceContents(RectTransform panel)
        {
            AddText("WORKSPACE", panel, 14, Color.white,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(12, -38), new Vector2(-12, -8),
                TextAnchor.MiddleLeft, FontStyle.Bold);

            AddText("ブロックを追加し、↑ ↓ で実行順を変更できます",
                panel, 11, new Color32(143, 163, 188, 255),
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(12, -60), new Vector2(-12, -38),
                TextAnchor.MiddleLeft);

            RectTransform scrollRect = CreateRect(
                "BlockScroll", panel,
                Vector2.zero, Vector2.one,
                new Vector2(10, 10), new Vector2(-10, -68));

            Image scrollBg = scrollRect.gameObject.AddComponent<Image>();
            scrollBg.color = panelInnerColor;

            ScrollRect scroll = scrollRect.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 35f;

            RectTransform viewport = CreateRect(
                "Viewport", scrollRect,
                Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero);
            viewport.gameObject.AddComponent<RectMask2D>();

            RectTransform content = CreateRect(
                "Content", viewport,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 0), new Vector2(0, 0));
            content.pivot = new Vector2(0.5f, 1f);

            VerticalLayoutGroup listLayout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            listLayout.padding = new RectOffset(8, 8, 8, 8);
            listLayout.spacing = 8;
            listLayout.childAlignment = TextAnchor.UpperCenter;
            listLayout.childControlWidth = true;
            listLayout.childControlHeight = true;
            listLayout.childForceExpandWidth = true;
            listLayout.childForceExpandHeight = false;

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;
            blockList = content;
        }

        private void BuildInspectorContents(RectTransform panel)
        {
            AddText("OBJECT INSPECTOR", panel, 14, Color.white,
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(12, -38), new Vector2(-12, -8),
                TextAnchor.MiddleLeft, FontStyle.Bold);

            inspectorText = AddText("", panel, 12,
                new Color32(176, 195, 218, 255),
                new Vector2(0, 0), new Vector2(1, 1),
                new Vector2(12, 122), new Vector2(-12, -50),
                TextAnchor.UpperLeft);
            inspectorText.horizontalOverflow = HorizontalWrapMode.Wrap;
            inspectorText.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform note = CreatePanel(
                "InspectorNote", panel, panelInnerColor,
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(10, 10), new Vector2(-10, 104));

            AddText("実行対象のGameObjectは、上部の TARGET OBJECT で名前を指定します。\n\n移動・回転は時間をかけて補間し、位置指定・サイズ・表示切替は即時反映します。",
                note, 11, new Color32(143, 163, 188, 255),
                Vector2.zero, Vector2.one,
                new Vector2(10, 8), new Vector2(-10, -8),
                TextAnchor.UpperLeft);

            AddButton("Transformをリセット", panel, new Color32(75, 94, 119, 255),
                ResetTargetTransform,
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(10, 58), new Vector2(-10, 94));
        }

        private void ApplyResponsiveLayout()
        {
            if (canvas == null || palettePanel == null)
                return;

            mobileLayout = IsMobileScreen();

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.referenceResolution = mobileLayout
                ? new Vector2(900f, 1600f)
                : new Vector2(1440f, 900f);
            scaler.matchWidthOrHeight = mobileLayout ? 0f : 0.5f;

            if (mobileLayout)
            {
                palettePanel.anchorMin = new Vector2(0, 0.72f);
                palettePanel.anchorMax = new Vector2(1, 1);
                palettePanel.offsetMin = Vector2.zero;
                palettePanel.offsetMax = Vector2.zero;

                workspacePanel.anchorMin = Vector2.zero;
                workspacePanel.anchorMax = new Vector2(1, 0.70f);
                workspacePanel.offsetMin = Vector2.zero;
                workspacePanel.offsetMax = Vector2.zero;

                inspectorPanel.gameObject.SetActive(false);
                paletteGrid.constraintCount = 4;
            }
            else
            {
                palettePanel.anchorMin = new Vector2(0, 0);
                palettePanel.anchorMax = new Vector2(0.22f, 1);
                palettePanel.offsetMin = Vector2.zero;
                palettePanel.offsetMax = Vector2.zero;

                workspacePanel.anchorMin = new Vector2(0.23f, 0);
                workspacePanel.anchorMax = new Vector2(0.77f, 1);
                workspacePanel.offsetMin = Vector2.zero;
                workspacePanel.offsetMax = Vector2.zero;

                inspectorPanel.anchorMin = new Vector2(0.78f, 0);
                inspectorPanel.anchorMax = new Vector2(1, 1);
                inspectorPanel.offsetMin = Vector2.zero;
                inspectorPanel.offsetMax = Vector2.zero;
                inspectorPanel.gameObject.SetActive(true);
                paletteGrid.constraintCount = 2;
            }

            Canvas.ForceUpdateCanvases();

            float panelWidth = palettePanel.rect.width;
            float usableWidth = Mathf.Max(120f, panelWidth - 20f);
            int columns = mobileLayout ? 4 : 2;
            float cellWidth = (usableWidth - (columns - 1) * paletteGrid.spacing.x) / columns;
            paletteGrid.cellSize = new Vector2(cellWidth, 38f);

            UpdateInspector();
        }

        private void AddPaletteButton(string label, VisualBlockType type, RectTransform parent)
        {
            AddButton(label, parent, GetBlockColor(type), () =>
            {
                program.blocks.Add(CreateDefaultBlock(type));
                RefreshBlocks();
                SetStatus(label + " ブロックを追加しました");
            }, new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero);
        }

        private VisualBlockData CreateDefaultBlock(VisualBlockType type)
        {
            bool isScale = type == VisualBlockType.SetScale;
            return new VisualBlockData
            {
                type = type,
                x = 1f,
                y = isScale ? 1f : 0f,
                z = isScale ? 1f : 0f,
                duration = 1f,
                visible = true,
                message = "Hello Unity!"
            };
        }

        private void RefreshBlocks()
        {
            if (blockList == null)
                return;

            for (int i = blockList.childCount - 1; i >= 0; i--)
                Destroy(blockList.GetChild(i).gameObject);

            if (program.blocks == null)
                program.blocks = new List<VisualBlockData>();

            if (program.blocks.Count == 0)
            {
                Text hint = AddText(
                    "まだブロックがありません。\n左の BLOCKS からブロックを追加してください。",
                    blockList, 14, new Color32(135, 155, 181, 255),
                    Vector2.zero, Vector2.one,
                    new Vector2(12, -90), new Vector2(-12, 0),
                    TextAnchor.MiddleCenter);
                LayoutElement hintLayout = hint.gameObject.AddComponent<LayoutElement>();
                hintLayout.preferredHeight = 90;
                hintLayout.minHeight = 90;
                hintLayout.flexibleWidth = 1;
                return;
            }

            for (int i = 0; i < program.blocks.Count; i++)
                CreateBlockCard(i);

            Canvas.ForceUpdateCanvases();
        }

        private void CreateBlockCard(int index)
        {
            VisualBlockData data = program.blocks[index];

            RectTransform card = CreatePanel(
                "Block_" + index, blockList, GetBlockColor(data.type),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            VerticalLayoutGroup cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            cardLayout.padding = new RectOffset(9, 9, 7, 7);
            cardLayout.spacing = 5;
            cardLayout.childControlWidth = true;
            cardLayout.childControlHeight = true;
            cardLayout.childForceExpandWidth = true;
            cardLayout.childForceExpandHeight = false;

            LayoutElement cardElement = card.gameObject.AddComponent<LayoutElement>();
            cardElement.minHeight = 86;
            cardElement.preferredHeight = 86;

            RectTransform titleRow = CreateLayoutRow("TitleRow", card, 30, 5);
            Text number = AddText((index + 1).ToString("00"), titleRow, 11,
                new Color32(180, 200, 225, 255),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                TextAnchor.MiddleLeft, FontStyle.Bold);
            SetPreferredWidth(number.gameObject, 28);

            Text title = AddText(GetBlockLabel(data.type), titleRow, 13, Color.white,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                TextAnchor.MiddleLeft, FontStyle.Bold);
            LayoutElement titleElement = title.gameObject.AddComponent<LayoutElement>();
            titleElement.flexibleWidth = 1;

            AddSmallActionButton("↑", titleRow, () => MoveBlock(index, -1), 34);
            AddSmallActionButton("↓", titleRow, () => MoveBlock(index, 1), 34);
            AddSmallActionButton("複製", titleRow, () => DuplicateBlock(index), 52);
            AddSmallActionButton("削除", titleRow, () => RemoveBlock(index), 52, dangerColor);

            RectTransform valueRow = CreateLayoutRow("ValuesRow", card, 32, 4);

            switch (data.type)
            {
                case VisualBlockType.Move:
                case VisualBlockType.Rotate:
                case VisualBlockType.SetPosition:
                case VisualBlockType.SetScale:
                    AddValueInput("X", data.x, valueRow, v => data.x = v);
                    AddValueInput("Y", data.y, valueRow, v => data.y = v);
                    AddValueInput("Z", data.z, valueRow, v => data.z = v);
                    if (data.type == VisualBlockType.Move || data.type == VisualBlockType.Rotate)
                        AddValueInput("秒", data.duration, valueRow,
                            v => data.duration = Mathf.Max(0.01f, v));
                    break;

                case VisualBlockType.Wait:
                    AddValueInput("秒", data.duration, valueRow,
                        v => data.duration = Mathf.Max(0f, v));
                    break;

                case VisualBlockType.SetVisible:
                    AddSmallActionButton(
                        data.visible ? "表示する" : "非表示にする",
                        valueRow,
                        () =>
                        {
                            data.visible = !data.visible;
                            RefreshBlocks();
                        }, 110, new Color32(71, 111, 150, 255));
                    break;

                case VisualBlockType.DebugLog:
                    InputField message = CreateInputField(
                        CreateRect("MessageInput", valueRow,
                            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero),
                        data.message, 12);
                    message.onValueChanged.AddListener(v => data.message = v);
                    LayoutElement messageLayout = message.gameObject.AddComponent<LayoutElement>();
                    messageLayout.flexibleWidth = 1;
                    messageLayout.minWidth = 120;
                    break;
            }
        }

        private RectTransform CreateLayoutRow(string name, RectTransform parent, float height, int spacing)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            HorizontalLayoutGroup layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;

            LayoutElement element = go.AddComponent<LayoutElement>();
            element.minHeight = height;
            element.preferredHeight = height;
            element.flexibleWidth = 1;

            return go.GetComponent<RectTransform>();
        }

        private void AddValueInput(string label, float value, RectTransform parent, Action<float> changed)
        {
            Text labelText = AddText(label, parent, 11,
                new Color32(180, 199, 220, 255),
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                TextAnchor.MiddleCenter);
            SetPreferredWidth(labelText.gameObject, 18);

            InputField input = CreateInputField(
                CreateRect("ValueInput_" + label, parent,
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero),
                value.ToString("0.##", CultureInfo.InvariantCulture), 12);

            LayoutElement inputLayout = input.gameObject.AddComponent<LayoutElement>();
            inputLayout.preferredWidth = 66;
            inputLayout.minWidth = 52;
            inputLayout.minHeight = 30;

            input.contentType = InputField.ContentType.DecimalNumber;
            input.onEndEdit.AddListener(text =>
            {
                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
                    changed(parsed);
            });
        }

        private void MoveBlock(int index, int offset)
        {
            int target = index + offset;
            if (target < 0 || target >= program.blocks.Count)
                return;

            VisualBlockData temp = program.blocks[index];
            program.blocks[index] = program.blocks[target];
            program.blocks[target] = temp;
            RefreshBlocks();
        }

        private void DuplicateBlock(int index)
        {
            if (index < 0 || index >= program.blocks.Count)
                return;

            VisualBlockData source = program.blocks[index];
            VisualBlockData copy = new VisualBlockData
            {
                type = source.type,
                x = source.x,
                y = source.y,
                z = source.z,
                duration = source.duration,
                message = source.message,
                visible = source.visible
            };

            program.blocks.Insert(index + 1, copy);
            RefreshBlocks();
        }

        private void RemoveBlock(int index)
        {
            if (index < 0 || index >= program.blocks.Count)
                return;

            program.blocks.RemoveAt(index);
            RefreshBlocks();
        }

        private void RunProgram()
        {
            program.targetObjectName = targetField.text.Trim();
            if (string.IsNullOrWhiteSpace(program.targetObjectName))
            {
                SetStatus("対象GameObject名を入力してください");
                return;
            }

            GameObject target = GameObject.Find(program.targetObjectName);
            if (target == null)
            {
                SetStatus("対象が見つかりません: " + program.targetObjectName);
                Debug.LogWarning("[VisualProgramming] Target not found: " + program.targetObjectName);
                UpdateInspector();
                return;
            }

            runner.Run(program, target.transform);
            SetStatus("実行中  •  " + program.blocks.Count + " blocks");
            UpdateInspector();
        }

        private void StopProgram()
        {
            if (runner != null)
                runner.Stop();

            SetStatus("実行を停止しました");
        }

        private void ResetTargetTransform()
        {
            GameObject target = FindTarget();
            if (target == null)
            {
                SetStatus("対象GameObjectが見つかりません");
                return;
            }

            target.transform.position = Vector3.zero;
            target.transform.rotation = Quaternion.identity;
            target.transform.localScale = Vector3.one;
            SetStatus("Transformを初期値に戻しました");
            UpdateInspector();
        }

        private void SaveProgram()
        {
            program.targetObjectName = targetField.text.Trim();
            string path = GetSavePath();

            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(program, true));
                SetStatus("保存しました  •  visual_program.json");
                Debug.Log("[VisualProgramming] Saved: " + path);
            }
            catch (Exception e)
            {
                SetStatus("保存に失敗しました: " + e.Message);
                Debug.LogException(e);
            }
        }

        private void LoadProgram()
        {
            string path = GetSavePath();
            if (!File.Exists(path))
            {
                SetStatus("保存ファイルがありません");
                return;
            }

            try
            {
                VisualProgramData loaded = JsonUtility.FromJson<VisualProgramData>(
                    File.ReadAllText(path));

                if (loaded == null)
                    throw new Exception("JSONの内容を読み取れません");

                program = loaded;
                program.blocks ??= new List<VisualBlockData>();
                targetField.text = program.targetObjectName ?? "Cube";

                RefreshBlocks();
                UpdateInspector();
                SetStatus("読み込みました  •  " + program.blocks.Count + " blocks");
            }
            catch (Exception e)
            {
                SetStatus("読み込みに失敗しました: " + e.Message);
                Debug.LogException(e);
            }
        }

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, "visual_program.json");
        }

        private GameObject FindTarget()
        {
            if (targetField == null || string.IsNullOrWhiteSpace(targetField.text))
                return null;

            return GameObject.Find(targetField.text.Trim());
        }

        private void UpdateInspector()
        {
            if (inspectorText == null)
                return;

            GameObject target = FindTarget();
            if (target == null)
            {
                inspectorText.text =
                    "対象: 未検出\n\n" +
                    "上部の入力欄に、Scene内のGameObject名を入力してください。\n\n" +
                    "例: Cube\n\n" +
                    "※ GameObject.Findで取得するため、アクティブなオブジェクトが対象です。";
                return;
            }

            Transform t = target.transform;
            inspectorText.text =
                "Name\n" + target.name + "\n\n" +
                "Position\n" + FormatVector(t.position) + "\n\n" +
                "Rotation\n" + FormatVector(t.eulerAngles) + "\n\n" +
                "Scale\n" + FormatVector(t.localScale) + "\n\n" +
                "Active\n" + target.activeInHierarchy;
        }

        private string FormatVector(Vector3 value)
        {
            return string.Format(CultureInfo.InvariantCulture,
                "X {0:0.00}\nY {1:0.00}\nZ {2:0.00}", value.x, value.y, value.z);
        }

        private string GetBlockLabel(VisualBlockType type)
        {
            switch (type)
            {
                case VisualBlockType.Move: return "移動する";
                case VisualBlockType.Rotate: return "回転する";
                case VisualBlockType.SetPosition: return "位置を指定";
                case VisualBlockType.SetVisible: return "表示を切り替え";
                case VisualBlockType.SetScale: return "サイズを変更";
                case VisualBlockType.Wait: return "待機する";
                case VisualBlockType.DebugLog: return "ログを出力";
                default: return type.ToString();
            }
        }

        private Color GetBlockColor(VisualBlockType type)
        {
            switch (type)
            {
                case VisualBlockType.Move:
                case VisualBlockType.Rotate:
                case VisualBlockType.SetPosition:
                case VisualBlockType.SetScale:
                    return new Color32(43, 91, 139, 255);
                case VisualBlockType.SetVisible:
                    return new Color32(91, 75, 137, 255);
                case VisualBlockType.Wait:
                    return new Color32(145, 101, 45, 255);
                case VisualBlockType.DebugLog:
                    return new Color32(54, 119, 100, 255);
                default:
                    return blockColor;
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        private void AddHeaderButton(
            string label, RectTransform parent, Color color,
            UnityEngine.Events.UnityAction action, float x, float width)
        {
            AddButton(label, parent, color, action,
                new Vector2(0, 0), new Vector2(0, 1),
                new Vector2(x, 7), new Vector2(x + width, -7));
        }

        private void AddSmallActionButton(
            string label, RectTransform parent,
            UnityEngine.Events.UnityAction action, float width,
            Color? color = null)
        {
            RectTransform rect = CreateRect(
                "Action_" + label, parent,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            Button button = rect.gameObject.AddComponent<Button>();
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color ?? new Color32(66, 85, 109, 255);
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            Text text = AddText(label, rect, 10, Color.white,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                TextAnchor.MiddleCenter);
            text.raycastTarget = false;

            LayoutElement element = rect.gameObject.AddComponent<LayoutElement>();
            element.preferredWidth = width;
            element.minWidth = width;
            element.minHeight = 28;
        }

        private Button AddButton(
            string label, RectTransform parent, Color color,
            UnityEngine.Events.UnityAction action,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = CreateRect(
                "Button_" + label, parent,
                anchorMin, anchorMax, offsetMin, offsetMax);

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);

            Text text = AddText(label, rect, 12, Color.white,
                Vector2.zero, Vector2.one,
                new Vector2(4, 2), new Vector2(-4, -2),
                TextAnchor.MiddleCenter, FontStyle.Bold);
            text.raycastTarget = false;

            return button;
        }

        private InputField CreateInputField(RectTransform rect, string value, int fontSize)
        {
            Image image = rect.gameObject.AddComponent<Image>();
            image.color = panelInnerColor;

            InputField input = rect.gameObject.AddComponent<InputField>();
            input.targetGraphic = image;
            input.contentType = InputField.ContentType.Standard;
            input.lineType = InputField.LineType.SingleLine;

            Text text = AddText(value, rect, fontSize, Color.white,
                Vector2.zero, Vector2.one,
                new Vector2(8, 2), new Vector2(-8, -2),
                TextAnchor.MiddleLeft);
            text.raycastTarget = false;
            input.textComponent = text;
            input.text = value;

            return input;
        }

        private Text AddText(
            string value, RectTransform parent, int size, Color color,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax,
            TextAnchor alignment, FontStyle style = FontStyle.Normal)
        {
            RectTransform rect = CreateRect(
                "Text", parent, anchorMin, anchorMax, offsetMin, offsetMax);

            Text text = rect.gameObject.AddComponent<Text>();
            text.font = uiFont;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.fontStyle = style;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.supportRichText = false;
            text.raycastTarget = false;

            return text;
        }

        private RectTransform CreatePanel(
            string name, Transform parent, Color color,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = CreateRect(
                name, parent, anchorMin, anchorMax, offsetMin, offsetMax);

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            return rect;
        }

        private RectTransform CreateRect(
            string name, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        private void SetPreferredWidth(GameObject go, float width)
        {
            LayoutElement element = go.GetComponent<LayoutElement>();
            if (element == null)
                element = go.AddComponent<LayoutElement>();

            element.preferredWidth = width;
            element.minWidth = width;
        }
    }
}
