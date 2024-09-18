﻿using System;
using System.Diagnostics.CodeAnalysis;
using Sisus.Init.EditorOnly.Internal;
using UnityEditor;
using UnityEngine;

namespace Sisus.Init.EditorOnly
{
	internal class DropdownButton
	{
		public const float ADD_ICON_WIDTH = 15f;

		internal readonly GUIContent prefixLabel;
		internal readonly GUIContent buttonLabel;
		private readonly Action<Rect> openDropdown;

		public DropdownButton(GUIContent prefixLabel, GUIContent buttonLabel, [DisallowNull] Action<Rect> openDropdown)
		{
			this.prefixLabel = prefixLabel;
			this.buttonLabel = buttonLabel;
			this.openDropdown = openDropdown;
		}

		public static GUIStyle TextStyle => EditorStyles.label;

		public void Draw(Rect position)
		{
			var buttonPosition = prefixLabel.text.Length > 0 ? EditorGUI.PrefixLabel(position, prefixLabel) : position;
            
			var iconRect = buttonPosition;
			iconRect.width = ADD_ICON_WIDTH;
			iconRect.x -= 1f;
			iconRect.y += 1f;

			var colorWas = GUI.color;
			GUI.color = Color.white;

			var clickableRect = iconRect;
			clickableRect.width += 4f;

			// this hack was needed to make the GUI.Button respond to clicks
			if(clickableRect.Contains(Event.current.mousePosition) && LayoutUtility.NowDrawing)
			{
				LayoutUtility.Repaint();
			}

			bool clicked = GUI.Button(clickableRect, GUIContent.none, EditorStyles.label);

			GUI.Label(iconRect, GUIContent.none, "PopupCurveDropdown");
			if(buttonLabel.text.Length > 0)
			{
				var labelRect = buttonPosition;
				labelRect.x += ADD_ICON_WIDTH + 4f;
				labelRect.width -= ADD_ICON_WIDTH;

				if(AnyPropertyDrawer.TryGetNullArgumentGuardBasedControlTint(out Color setGUIColor))
				{
					GUI.color = setGUIColor;
				}

				GUI.Label(labelRect, buttonLabel, TextStyle);
			}

			GUI.color = colorWas;
			if(clicked)
			{
				openDropdown(buttonPosition);
			}
		}

		public Rect DrawPrefixLabel(Rect position) => prefixLabel.text.Length > 0 ? EditorGUI.PrefixLabel(position, prefixLabel) : position;

		public void OpenDropdown(Rect buttonPosition) => openDropdown(buttonPosition);
	}
}