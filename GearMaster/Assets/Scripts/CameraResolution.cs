using UnityEngine;
using System.Collections;

public class CameraResolution : MonoBehaviour {

	public int renderHeight = 128;
	public bool halve = false;
	public Camera whichCamera;

	RenderTexture renderTexture;

	public Material renderedMaterial;
	public Material postEffectMaterial;

	// Use this for initialization
	void Start () {
		float ratio = (float)Screen.width / (float)Screen.height;
		int renderWidth = Mathf.RoundToInt ((float)renderHeight * ratio);
		if (halve) {
			renderWidth = renderWidth / 2;
		}

		renderTexture = new RenderTexture(renderWidth, renderHeight, 16);
		renderTexture.isPowerOfTwo = renderWidth%2 == 0 && renderHeight%2 == 0;
		renderTexture.anisoLevel = 0;
		renderTexture.filterMode = FilterMode.Point;
		if (!renderTexture.IsCreated()) {
			renderTexture.Create();
		}

		Camera cam = whichCamera;
		if (cam == null) {
			cam = Camera.main;
		}
		cam.targetTexture = renderTexture;
		renderedMaterial.mainTexture = renderTexture;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit (source, destination, postEffectMaterial);
	}
}
