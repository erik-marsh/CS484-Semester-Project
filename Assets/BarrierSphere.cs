using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSphere : MonoBehaviour
{
	public GameObject barrierSphere;

	private void Awake()
	{
		// invert sphere normals so we can texture the inside
		Vector3[] normals = barrierSphere.GetComponent<MeshFilter>().mesh.normals;
		for (int i =0; i < normals.Length; i++)
		{
			normals[i] = -normals[i];
		}
		barrierSphere.GetComponent<MeshFilter>().mesh.normals = normals;
	}
}
