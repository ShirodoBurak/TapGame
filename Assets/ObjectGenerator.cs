using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectGenerator : MonoBehaviour{
	int ObjectCount;
	[Range(2, 10)] public int maxObjects;
	public GameObject hittableObject;

	private float[] x = new float[10000];
	private float[] y = new float[10000];
	private float[] r = new float[10000];
	private float[] g = new float[10000];
	private float[] b = new float[10000];

	void Start()
	{
		for (int i = 0; i < 10000; i++)
		{
			//x[i] = Random.Range(-3.5f, 3.5f);
			//y[i] = Random.Range(-3.5f, 3.5f);
			x[i] = Random.Range(0f, 0f);
			y[i] = Random.Range(0f, 0f);
			r[i] = Random.Range(0.5f, 1f);
			g[i] = Random.Range(0.5f, 1f);
			b[i] = Random.Range(0.5f, 1f);
		}
	}

	void Update()
	{
		if (ObjectCount < maxObjects)
		{
			int index = Random.Range(0, 10000);

			hittableObject.GetComponent<SpriteRenderer>().color = new Color(r[index], g[index], b[index]);
			hittableObject.GetComponent<Transform>().rotation =
				Quaternion.Euler(Random.Range(0, 90), Random.Range(0, 90), 0);

			Instantiate(hittableObject, new Vector3(x[index], y[index], -1), Quaternion.identity);
			increaseCount();
		}
	}

	public void decreaseCount()
	{
		ObjectCount--;
	}

	public void increaseCount()
	{
		ObjectCount++;
	}
}