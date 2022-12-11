using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(TextMeshProUGUI))]
public class TextInputReplacement : MonoBehaviour {
	public InputActionReference[] inputs;

	private void Awake() {
		TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

		for (int i = 0; i < inputs.Length; i++) {
			text.text = text.text.Replace("{{" + i + "}}", inputs[i].action.GetBindingDisplayString());
		}
	}
}