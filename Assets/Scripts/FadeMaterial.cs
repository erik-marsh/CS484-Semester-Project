using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour
{
	public GameObject thisObject;

	private Renderer renderer;
	private Material currMaterial;
	private Material targetMaterial;
	private float fadeTime;
	private float fadeStartTime;
	private bool isFading;

	private void Awake()
	{
		renderer = thisObject.GetComponent<Renderer>();
		currMaterial = renderer.material;
		isFading = false;
	}

	private void Update()
	{
		if (isFading)
		{
			if (Time.time >= fadeStartTime + fadeTime)
			{
				isFading = false;
				renderer.material = targetMaterial;
				currMaterial = renderer.material;
			}
			else
			{
				float lerp = (Time.time - fadeStartTime) / fadeTime;
				renderer.material.Lerp(currMaterial, targetMaterial, lerp);
			}
		}
	}

	public void ChangeMaterial(Material targetMaterial, float fadeTime)
	{
		this.targetMaterial = targetMaterial;
		this.fadeTime = fadeTime;
		fadeStartTime = Time.time;

		isFading = true;
	}
}
