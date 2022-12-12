using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPortal : InteractableObject {
	public AudioClip pickUpSound;
	
	public override void PrimaryInteract() {
		LevelManager.Instance.CanUsePortal = true;
		playerController.handheldPortal.ShowPortal();
		playerController.handheldPortal.UpdateCollisions();
		
		playerController.footstepSource.PlayOneShot(pickUpSound);
		
		Destroy(gameObject);
	}
}
