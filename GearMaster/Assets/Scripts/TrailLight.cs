using UnityEngine;
using System.Collections;

public class TrailLight : MonoBehaviour {

	public Light light;
	public MeshRenderer mesh;
	public TrailRenderer trail;

	float baseIntensity = 0f;
	float baseStartWidth = 0f;

	// Use this for initialization
	void Start () {
		baseIntensity = light.intensity;
		baseStartWidth = trail.startWidth;
		SetIntensity (0f);
	}

	public void SetIntensity (float i) {
		float eased = Easing.Cubic.In (i);
		light.intensity = baseIntensity * eased;
		mesh.material.color = Color.Lerp (Color.black, Color.white, eased);
		trail.startWidth = baseStartWidth * eased;
	}

	public void SetTrail ( bool enabled ) {
		trail.enabled = enabled;
	}
}
