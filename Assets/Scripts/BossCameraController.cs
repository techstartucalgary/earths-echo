using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraController : MonoBehaviour
{
	public float zoomOutSize = 12f;
	public float zoomSpeed = 2f;

	private Camera mainCam;
	private float originalSize;
	private bool shouldZoomOut = false;
	private bool shouldZoomIn = false;

    // Start is called before the first frame update
    void Start()
    {
		mainCam = Camera.main;
		originalSize = mainCam.orthographicSize;
    }

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Player")) {
			shouldZoomOut = true;
			shouldZoomIn = false;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if(other.CompareTag("Player")) {
			shouldZoomOut = false;
			shouldZoomIn = true;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if(shouldZoomOut) {
			mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, zoomOutSize, Time.deltaTime * zoomSpeed);

		}
		else if(shouldZoomIn) {
			mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, originalSize, Time.deltaTime * zoomSpeed);
		}
    }
}
