using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalHandler : MonoBehaviour {
	public static event Action OnLevelComplete;

	private PlayerController _playerController;

	private void Awake() {
		_playerController = PlayerController.Instance;
	}

	private void OnTriggerEnter(Collider other) {
		if (other == _playerController.GetComponent<Collider>()) OnLevelComplete?.Invoke();
	}
}
