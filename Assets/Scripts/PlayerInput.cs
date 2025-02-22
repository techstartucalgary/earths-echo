using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;

	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {

        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

        // Check for sprint input (Left Shift key)
        player.SetSprinting(Input.GetKey(KeyCode.LeftShift));

        // Sliding logic
        if (Input.GetKeyDown(KeyCode.C) && player.isSprinting)
        {
            player.StartSlide();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            player.StopSlide();
        }
        
        // Attacks
        if (Input.GetMouseButtonDown(0)) {
            if (Input.GetKey(KeyCode.W)) {
                Debug.Log("Up Attack Triggered");
                player.PerformUpAttack(5, 0.5f);
            }
            else if (Input.GetKey(KeyCode.S)) {
                Debug.Log("Down Attack Triggered");
                player.PerformDownAttack(5, 0.5f);
            }
            else {
                Debug.Log("Side Attack Triggered");
                player.PerformSideAttack(5, 0.5f);
            }
        }


		player.playerSpeed = player.velocity.x; // So we can see the current speed
    }
}
