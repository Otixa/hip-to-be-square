using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Sprites;

[CustomEditor(typeof(TileSet))]
public class TilesetEditor: Editor
{
    TileSet t;

    GameObject go_tmp;
    int selection;
    int controlID;

    Dictionary<string, Texture2D> cachedTextures;

    public void OnEnable()
    {
        try
        {
            if (target != null)
            {
                t = (TileSet)target;
                if (t.Tiles == null)
                {
                    t.Tiles = new List<GameObject>();
                }
            }
        }
        catch (IndexOutOfRangeException) { }
        cachedTextures = new Dictionary<string, Texture2D>();
    }

    void AddButton()
    {
        if (GUILayout.Button("Add"))
        {
            controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "", controlID);
        }
        string commandName = Event.current.commandName;
        if (commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == controlID)
        {
            go_tmp = (GameObject)EditorGUIUtility.GetObjectPickerObject();
            controlID = -1;
            Repaint();
        }
        else if (commandName == "ObjectSelectorClosed")
        {
            if (go_tmp != null && !t.Tiles.Contains(go_tmp))
            {
                if (go_tmp.GetComponent<Collider2D>() != null)
                {
                    t.Tiles.Add(go_tmp);
                    EditorUtility.SetDirty(t);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error adding Tile", "Prefab does not contain a Collider2D.", "Ok");
                }
            }
        }
    }

    GUIContent[] getTextures()
    {
        GUIContent[] co = new GUIContent[t.Tiles.Count];
        for (int i = 0; i < co.Length; i++)
        {
            GUIContent c = new GUIContent();
            Sprite sprite = t.Tiles[i].GetComponent<SpriteRenderer>().sprite;
            if (cachedTextures.ContainsKey(sprite.name))
            {
                c.image = cachedTextures[sprite.name];
            }
            else
            {
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
                    cachedTextures.Add(sprite.name, newTex);
                }
                catch (UnityException)
                {
                    c.image = sprite.texture;
                    cachedTextures.Add(sprite.name, sprite.texture);
                }
            }
            c.tooltip = t.Tiles[i].name;
            co[i] = c;
        }
        return co;
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

    public override void OnInspectorGUI()
    {
        if (t.Tiles.Count > 0)
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

        GUILayout.BeginHorizontal();
        AddButton();

        if (t.Tiles.Count < 1) GUI.enabled = false;
        if (GUILayout.Button("Remove"))
        {
            t.Tiles.RemoveAt(selection);
            EditorUtility.SetDirty(t);
        }
        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }
}
