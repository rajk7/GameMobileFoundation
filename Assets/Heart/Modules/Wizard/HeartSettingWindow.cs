﻿using System;
using System.IO;
using Pancake;
using Pancake.Common;
using PancakeEditor.Common;
using UnityEditor;
using UnityEngine;

namespace PancakeEditor
{
    internal static class HeartSettingWindow
    {
        public static void OnInspectorGUI()
        {
            var heartSetting = Resources.Load<HeartSettings>(nameof(HeartSettings));
            var heartEditorSetting = Resources.Load<HeartEditorSettings>(nameof(HeartEditorSettings));
            if (heartSetting == null || heartEditorSetting == null)
            {
                GUI.enabled = !EditorApplication.isCompiling;
                GUI.backgroundColor = Uniform.Pink;
                if (GUILayout.Button("Create Missing Heart Setting", GUILayout.MaxHeight(Wizard.BUTTON_HEIGHT)))
                {
                    if (heartSetting == null)
                    {
                        var setting = ScriptableObject.CreateInstance<HeartSettings>();
                        if (!Directory.Exists(Common.Editor.DEFAULT_RESOURCE_PATH)) Directory.CreateDirectory(Common.Editor.DEFAULT_RESOURCE_PATH);
                        AssetDatabase.CreateAsset(setting, $"{Common.Editor.DEFAULT_RESOURCE_PATH}/{nameof(HeartSettings)}.asset");
                        Debug.Log($"{nameof(HeartSettings).SetColor("f75369")} was created ad {Common.Editor.DEFAULT_RESOURCE_PATH}/{nameof(HeartSettings)}.asset");
                    }

                    if (heartEditorSetting == null)
                    {
                        var editorSetting = ScriptableObject.CreateInstance<HeartEditorSettings>();
                        if (!Directory.Exists(Common.Editor.DEFAULT_EDITOR_RESOURCE_PATH)) Directory.CreateDirectory(Common.Editor.DEFAULT_EDITOR_RESOURCE_PATH);
                        AssetDatabase.CreateAsset(editorSetting, $"{Common.Editor.DEFAULT_EDITOR_RESOURCE_PATH}/{nameof(HeartEditorSettings)}.asset");
                        Debug.Log(
                            $"{nameof(HeartEditorSettings).SetColor("f75369")} was created ad {Common.Editor.DEFAULT_EDITOR_RESOURCE_PATH}/{nameof(HeartEditorSettings)}.asset");
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                GUI.backgroundColor = Color.white;
                GUI.enabled = true;
            }
            else
            {
                var editorHeartSetting = UnityEditor.Editor.CreateEditor(heartSetting);
                editorHeartSetting.OnInspectorGUI();
                EditorGUILayout.Space();
                var editorHeartEditorSetting = UnityEditor.Editor.CreateEditor(heartEditorSetting);
                editorHeartEditorSetting.OnInspectorGUI();
            }
        }
    }
}