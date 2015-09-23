using UnityEngine;
using System.Collections;

public class WorldCanvasScaler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float ratio = (float)Screen.width / (float)Screen.height;

		GetComponent<RectTransform> ().sizeDelta = new Vector2 (GetComponent<RectTransform> ().sizeDelta.y * ratio, GetComponent<RectTransform> ().sizeDelta.y);
	}
}
