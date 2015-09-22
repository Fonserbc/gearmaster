using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {


	Vector3 originalPos;
	float speed = 15f;

	// Use this for initialization
	void Start () {
		originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (Vector3.zero, Vector3.up, Time.deltaTime * speed);
	}
}
