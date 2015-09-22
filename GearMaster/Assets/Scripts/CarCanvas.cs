using UnityEngine;
using System.Collections;

public class CarCanvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float ratio = Screen.width / 2f / Screen.height;
		RectTransform rt = GetComponent<RectTransform> ();
		rt.sizeDelta = new Vector2 (rt.sizeDelta.y * ratio, rt.sizeDelta.y);
	}
}
