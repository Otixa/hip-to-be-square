using System;

using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

[Serializable]
public class LevelEditor2D: EditorWindow
{
    [SerializeField]
    TileSet ts;
    TileSet ts_tmp;
    bool showGridEditor = true;
    bool showTileSetEditor = true;

    bool showGrid = true;
    int selection = 0;
    [SerializeField]
    bool drawMode = false;

    bool shouldRepaint = false;
    [SerializeField]
    float gridSize = 1.5f;
    [SerializeField]
    Color gridColor = Color.red;
    [SerializeField]
    LayerMask drawLayer;

    Vector2 screenMin;
    Vector2 screenMax;

    [MenuItem("2D Level/2D Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor2D>("2D Level Editor");
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void DrawGridEditor()
    {
        EditorGUI.indentLevel++;
        if (EditorGUILayout.Toggle("Show Grid:", showGrid) != showGrid)
        {
            showGrid = !showGrid;
            shouldRepaint = true;
        }
        var tGs = EditorGUILayout.Slider("Grid Size:", gridSize, 0.1f, 10f);
        if (tGs != gridSize)
        {
            gridSize = tGs;
            shouldRepaint = true;
        }
        Color tGc = EditorGUILayout.ColorField("Grid Color:", gridColor);
        if (tGc != gridColor)
        {
            gridColor = tGc;
            shouldRepaint = true;
        }
        if (showGrid) ToggleGridUtility.ShowGrid = false;
        EditorGUI.indentLevel--;
    }

    void DrawTilesetEditor()
    {
        EditorGUI.indentLevel++;
        drawMode = EditorGUILayout.Toggle("Draw Mode:", drawMode);
        drawLayer = EditorGUILayout.LayerField("Draw Layer:", drawLayer);
        EditorGUILayout.Space();
        if (GUILayout.Button("Select TileSet"))
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<TileSet>(null, false, "", controlID);
        }
        string commandName = Event.current.commandName;
        if (commandName == "ObjectSelectorUpdated")
        {
            ts_tmp = (TileSet)EditorGUIUtility.GetObjectPickerObject();
            Repaint();
        }
        else if (commandName == "ObjectSelectorClosed")
        {
            if (ts_tmp != null)
            {
                ts = ts_tmp;
            }
        }
        if (ts != null && ts.Tiles.Count > 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            float s = Screen.width;
            GUIStyle style = new GUIStyle("button");
            style.margin = new RectOffset(1, 1, 1, 1);
            style.padding = new RectOffset(2, 2, 2, 2);
            style.fixedWidth = 64f;
            style.fixedHeight = 64f;
            style.stretchHeight = true;
            style.stretchWidth = true;
            style.overflow = new RectOffset(1, 1, 1, 1);
            style.normal.background = MakeTex(64, 64, Color.white);
            style.onNormal.background = MakeTex(64, 64, Color.black);
            selection = GUILayout.SelectionGrid(selection, getTextures(), Mathf.FloorToInt(s / 70), style);
            EditorGUILayout.EndVertical();
        }
        EditorGUI.indentLevel--;
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    GUIContent[] getTextures()
    {
        GUIContent[] co = new GUIContent[ts.Tiles.Count];
        for (int i = 0; i < co.Length; i++)
        {
            GUIContent c = new GUIContent();
            Sprite sprite = ts.Tiles[i].GetComponent<SpriteRenderer>().sprite;
            try
            {
                Rect texRect = sprite.textureRect;
                Color[] originalSprite = SpriteUtility.GetSpriteTexture(sprite, false).GetPixels(
                    (int)texRect.x,
                    (int)texRect.y,
                    (int)texRect.width,
                    (int)texRect.height);
                Texture2D newTex = new Texture2D((int)texRect.width, (int)texRect.height);
                newTex.SetPixels(originalSprite);
                c.image = newTex;
            }
            catch (UnityException)
            {
                c.image = sprite.texture;
            }
            c.tooltip = ts.Tiles[i].name;
            co[i] = c;
        }
        return co;
    }

    void OnGUI()
    {
        showGridEditor = EditorGUILayout.Foldout(showGridEditor, "Grid Settings");
        if (showGridEditor)
        {
            DrawGridEditor();
        }
        EditorGUILayout.Space();
        showTileSetEditor = EditorGUILayout.Foldout(showTileSetEditor, "Tileset Settings");
        if (showTileSetEditor)
        {
            DrawTilesetEditor();
        }
    }

    void OnSceneGUI(SceneView sceneView)
    {
        int controlID = GUIUtility.GetControlID(666, FocusType.Passive);
        if (drawMode)
        {
            HandleUtility.AddDefaultControl(controlID);
            Tools.current = Tool.None;
        }

        UpdateCameraBounds();
        Color originalColor = Handles.color;
        Handles.color = gridColor;

        if (Event.current.isMouse)
        {
            HandleMouseEvents();
        }

        if (showGrid) DrawGrid();
        if (shouldRepaint) HandleUtility.Repaint();
        Handles.color = originalColor;
    }

    void HandleMouseEvents()
    {
        if (!drawMode) return;
        Event e = Event.current;
        Vector3 mpos = Event.current.mousePosition;
        mpos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mpos.y;
        mpos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mpos);
        mpos.z = 0;

        mpos.x = FindNearest(mpos.x);
        mpos.y = FindNearest(mpos.y);

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                if (e.button == 0 && drawMode && ts != null)
                {
                    PlaceAtMousePos(mpos);
                    e.Use();
                }
                else if (e.button == 1 && drawMode)
                {
                    GameObject go = GetAtMousePosition();
                    if (go != null)
                    {
                        DestroyImmediate(go);
                    }
                    e.Use();
                }
                break;
        }
    }

    void PlaceAtMousePos(Vector2 pos)
    {
        GameObject go = GetAtMousePosition();
        if (go != null)
        {
            DestroyImmediate(go);
        }
        go = Instantiate(ts.Tiles[selection], pos, Quaternion.identity);
        go.layer = drawLayer.value;
        go.name = go.name.Replace("(Clone)", "_2DLgenAuto");
        go.hideFlags = HideFlags.HideInHierarchy;
    }

    GameObject GetAtMousePosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, Vector2.zero, Mathf.Infinity);
        if (hits.Length == 0)
        {
            return null;
        }
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.name.Contains("_2DLgenAuto"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    void UpdateCameraBounds()
    {
        if (screenMin != (Vector2)Camera.current.ScreenToWorldPoint(Vector2.zero))
        {
            screenMin = Camera.current.ScreenToWorldPoint(Vector2.zero);
            shouldRepaint = true;
        }
        if (screenMax != (Vector2)Camera.current.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)))
        {
            screenMax = Camera.current.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            shouldRepaint = true;
        }
    }

    float FindNearest(float max, float offset = 0)
    {
        float t = max - offset;
        t = max / gridSize;
        t = Mathf.Round(t);
        t *= gridSize;
        t += offset;
        return t;
    }

    void DrawGrid()
    {

        float minX = FindNearest(Mathf.Floor(screenMin.x), gridSize * .5f) - gridSize;
        float maxX = FindNearest(Mathf.Ceil(screenMax.x), gridSize * .5f) + gridSize;
        float minY = FindNearest(Mathf.Floor(screenMin.y), gridSize * .5f) - gridSize;
        float maxY = FindNearest(Mathf.Ceil(screenMax.y), gridSize * .5f) + gridSize;
        for (float x = minX; x <= maxX; x += gridSize)
        {
            if (x < screenMin.x || x > screenMax.x)
            {
                continue;
            }
            Handles.DrawLine(
                new Vector3(x, minY, 0),
                new Vector3(x, maxY, 0));
        }

        for (float y = minY; y <= maxY; y += gridSize)
        {
            if (y < screenMin.y || y > screenMax.y)
            {
                continue;
            }
            Handles.DrawLine(
                new Vector3(minX, y, 0),
                new Vector3(maxX, y, 0));
        }
    }
}