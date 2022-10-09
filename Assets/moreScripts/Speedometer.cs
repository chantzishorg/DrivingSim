

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

/// <summary>
/// This is a really simple graphing solution for the WheelCollider's friction slips.
/// </summary> 
public class Speedometer : MonoBehaviour
{
	public Rigidbody vehicleBody;
	public float width = 0.35f;

	Text m_SpeedText;

	const string k_EventSystemName = "EventSystem";
	const string k_GraphCanvasName = "GraphCanvas";
	const string k_InfoTextName = "InfoText";
	const float k_GUIScreenEdgeOffset = 10f;
	const int k_InfoFontSize = 16;

	void Start()
	{
		// Add GUI infrastructure.
		var eventSystem = new GameObject(k_EventSystemName);
		eventSystem.AddComponent<EventSystem>();
		eventSystem.AddComponent<StandaloneInputModule>();

		var canvas = new GameObject(k_GraphCanvasName);
		var canvasScript = canvas.AddComponent<Canvas>();
		canvas.AddComponent<CanvasScaler>();
		canvas.AddComponent<GraphicRaycaster>();

		canvasScript.renderMode = RenderMode.ScreenSpaceOverlay;

		// Add speed textbox.
		var infoGo = new GameObject(k_InfoTextName);
		infoGo.transform.parent = canvas.transform;
		m_SpeedText = infoGo.AddComponent<Text>();
		var textXform = infoGo.GetComponent<RectTransform>();

		m_SpeedText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		m_SpeedText.fontSize = k_InfoFontSize;

		textXform.anchorMin = Vector2.up;
		textXform.anchorMax = Vector2.up;
		textXform.pivot = Vector2.up;
		textXform.anchoredPosition = new Vector2(k_GUIScreenEdgeOffset, -k_GUIScreenEdgeOffset);
		var rect = textXform.sizeDelta;
		rect.x = (int)(Screen.width * width);
		textXform.sizeDelta = rect;
	}

	void Update()
	{
		if (vehicleBody)
			m_SpeedText.text = string.Format("Speed: {0:0.} km/h", Mathf.Round(vehicleBody.velocity.magnitude * 3.6f));
	}
}