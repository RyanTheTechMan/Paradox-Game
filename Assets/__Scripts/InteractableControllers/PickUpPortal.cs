using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPortal : InteractableObject {
	public override void PrimaryInteract() {
		LevelManager.Instance.CanUsePortal = true;
		playerController.handheldPortal.isPortalActive = true;
		playerController.handheldPortal.UpdateCollisions();
		
		Destroy(gameObject);
	}
}
