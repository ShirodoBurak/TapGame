using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;
public class Hitter : MonoBehaviour{
	private bool collided;
	private bool comboCaroutineIsRunning = false;
	private int defaultNeededComboPoint = 2;
	private int Score;
	private int target;
	private float life;
	private bool dead = false;
	private float timeElapsed = 0f;
	private int comboPoint;
	private int ComboMultiplier;
	private GameObject collidedObject;
	[Header("Game Objects")] public ObjectGenerator objGen;
	public GameObject LifeBar;
	public GameObject ScoreBar;
	public GameObject TimeBar;
	public GameObject comboBar;
	public GameObject MultiplierText;
	public GameObject[] SideBars = new GameObject[2];

	[Header("Game options")] [Range(1, 100)]
	public float maxLife;

	[Range(1000,2500)]public int maxOverLife;
	[Range(1, 10)] public float drainAmount;
	[Range(1, 12)] public int maxCombo;
	[Range(0.1f, 5f)] public float maxDrain;
	[Header("Audio")] public AudioSource audioSource;
	public AudioClip sound;
	[Header("Animation")] public Animation hitAnimation;
	public Camera cam;
	[Header("Colors")]public Color[] comboColors = new Color[9];
	void Start(){
		target = defaultNeededComboPoint;
		comboBar.transform.localScale =
			new Vector3(comboBar.transform.localScale.x, 0, comboBar.transform.localScale.z);
		Cursor.visible = false;
		life = maxLife;
		audioSource.clip = sound;
		StartCoroutine(Counter());
		StartCoroutine(DrainLife());
	}
	void Update(){
		if (life>maxOverLife){
			life = maxOverLife;
		}
		Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
		this.gameObject.transform.position = new Vector3(pos.x, pos.y, -1);
		if (Input.GetMouseButtonDown(0)){
			StartCoroutine(SetComboBarSize());
			if (collided && !dead){
				if (life/maxLife>1){
					SideBars[0].GetComponent<ParticleSystem>().startColor = new Color(0.66f, 0.02f, 0.92f);
					SideBars[1].GetComponent<ParticleSystem>().startColor = new Color(0.66f, 0.02f, 0.92f);
					SideBars[0].GetComponent<ParticleSystem>().Play();
					SideBars[1].GetComponent<ParticleSystem>().Play();	
				}
				comboPoint++;
				if (!comboCaroutineIsRunning){
					StartCoroutine(ComboBar());
				}
				hitAnimation.Play();
				audioSource.pitch = Random.Range(1f, 5f);
				audioSource.Play();
				Destroy(collidedObject);
				objGen.decreaseCount();
				if (ComboMultiplier > 0){
					if (ComboMultiplier!=maxCombo){
						comboPoint++;	
					}else{
						comboPoint = 24;
					}
					if (life > maxLife){
						Score = Mathf.RoundToInt(Score + (300) * drainAmount * (ComboMultiplier) + life);
					}
					else{
						Score = Mathf.RoundToInt(Score + (300) * drainAmount * (ComboMultiplier));
					}
				}
				else{
					if (ComboMultiplier!=maxCombo){
						comboPoint++;
					}
					else{
						comboPoint = 24;
					}
					if (life > maxLife){
						Score = Mathf.RoundToInt(Score + (300) * drainAmount + life);
					}
					else{
						Score = Mathf.RoundToInt(Score + (300) * drainAmount);
					}
				}

				ScoreBar.gameObject.GetComponent<Text>().text = Score.ToString();
				life = life + maxLife / 7 * (ComboMultiplier/2);
			}
			else{
				ComboMultiplier = 0;
				comboPoint = 0;
				comboBar.transform.localScale =
					new Vector3(comboBar.transform.localScale.x, 0, comboBar.transform.localScale.z);
				MultiplierText.GetComponent<Text>().text = ComboMultiplier.ToString() + "x";
				MultiplierText.GetComponent<Text>().fontSize = 12 + ComboMultiplier;
				Debug.Log("Click, nothing.");
			}
		}
	}
	void setTime(){
		int mins = Mathf.FloorToInt(timeElapsed / 60);
		string seconds = Mathf.FloorToInt(timeElapsed - mins * 60).ToString();
		if (seconds.Length == 1){
			seconds = "0" + seconds;
		}
		TimeBar.GetComponent<Text>().text = mins + ":" + seconds;
	}
	IEnumerator Counter(){
		while (!dead){
			timeElapsed++;
			setTime();
			yield return new WaitForSeconds(1f);
		}
	}
	IEnumerator ComboBar(){
		int targetMax = 24;
		comboCaroutineIsRunning = true;
		target = defaultNeededComboPoint;
		while (comboPoint != 0){
			var a = comboBar.transform.localScale;
			if (comboPoint > target){
				if (target >= targetMax){
					target = targetMax;
					comboPoint = 1;
				}
				else{
					target = target * 2;
					comboPoint = 1;
				}
				if (ComboMultiplier!=maxCombo || ComboMultiplier<maxCombo){
					ComboMultiplier++;
				}
				if (ComboMultiplier==9){
					comboPoint = 24;
				}
				MultiplierText.GetComponent<Text>().text = ComboMultiplier.ToString() + "x";
				MultiplierText.GetComponent<Text>().fontSize = 12 + ComboMultiplier;
			}
			
			yield return new WaitForSeconds(0.01f);
		}
		comboCaroutineIsRunning = false;
	}
	IEnumerator SetComboBarSize(){
		
		var a = comboBar.transform.localScale;
		float startPoint = comboPoint;
		if (a.y != comboPoint / target * 100){
			if (a.y < comboPoint / target * 100){
				comboBar.transform.localScale = new Vector3(a.x, a.y + (startPoint / target) * 10, a.z);
				Debug.Log("Arttır");
			}
			if (a.y > comboPoint / target * 100){
				Debug.Log("Azalt");
				comboBar.transform.localScale = new Vector3(a.x, a.y + (target/startPoint) * 10, a.z);
			}
		}

		yield return new WaitForSeconds(0.5f);
		SetComboBarColor();
	}

	void SetComboBarColor(){
		if (ComboMultiplier!=0){
			comboBar.GetComponent<SpriteRenderer>().color = comboColors[ComboMultiplier-1];	
		}
	}
	IEnumerator DrainLife(){
		float lifeBarScaleX = LifeBar.transform.localScale.x;
		while (life > 0){
			if ((drainAmount + (timeElapsed / 6)) / 10 > maxDrain){
				life = life - maxDrain;
			}
			else{
				life = life - (drainAmount + (timeElapsed / 6)) / 10;
			}

			float LifePercentage = life / maxLife; //returns something between 0f and 1f. Dependends on the player hp
			if (life < maxLife){
				LifeBar.GetComponent<SpriteRenderer>().color = new Color(1, LifePercentage, LifePercentage);
			}
			else{
				LifeBar.GetComponent<SpriteRenderer>().color = new Color(0.66f, 0.02f, 0.92f);
			}
			if (LifePercentage > 1){
				LifeBar.transform.localScale = new Vector3(1f, LifeBar.transform.localScale.y, -1);
			}
			else{
				LifeBar.transform.localScale = new Vector3(LifePercentage, LifeBar.transform.localScale.y, -1);
			}
			yield return new WaitForSeconds(0.01f);
		}
		LifeBar.GetComponent<Animation>().Play();
		SideBars[0].GetComponent<ParticleSystem>().Play();
		SideBars[1].GetComponent<ParticleSystem>().Play();
		audioSource.pitch = Random.Range(0.8f, 1f);
		audioSource.Play();
		dead = true;
	}
	private void OnCollisionStay2D(Collision2D other){
		collided = true;
		collidedObject = other.gameObject;
	}
	private void OnCollisionExit2D(Collision2D other){
		collided = false;
		collidedObject = null;
	}
}