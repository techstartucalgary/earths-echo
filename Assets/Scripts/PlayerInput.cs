using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;

	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {
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

		player.playerSpeed = player.velocity.x; // So we can see the current speed
    }
}
