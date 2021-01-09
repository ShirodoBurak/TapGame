using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class Hitter : MonoBehaviour {
    private bool collided;
    private int Score;
    private float life;
    private bool dead = false;
    private float timeElapsed = 0f;
    private GameObject collidedObject;
    [Header("Game Objects")]
    public ObjectGenerator objGen;
    public GameObject LifeBar;
    public GameObject ScoreBar;
    public GameObject TimeBar;
    public GameObject[] SideBars = new GameObject[2];
    [Header("Game options")] [Range(1,100)]public float maxLife;[Range(1,10)]public float drainAmount;
    [Range(0.1f, 1f)] public float maxDrain;
    [Header("Audio")] public AudioSource audioSource; public AudioClip sound;
    [Header("Animation")] public Animation hitAnimation;public Camera cam;
    void Start() {
        Cursor.visible = false;
        life = maxLife;
        audioSource.clip = sound;
        StartCoroutine(Counter());
        StartCoroutine(DrainLife());
    }

    // Update is called once per frame
    void Update() {
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.transform.position = new Vector3(pos.x,pos.y,-1);
        if (Input.GetMouseButtonDown(0)) {
            if(collided && !dead) {
                hitAnimation.Play();
                audioSource.pitch = Random.Range(1f, 5f);
                audioSource.Play();
                Destroy(collidedObject);
                objGen.decreaseCount();
                if (life>maxLife)
                {
                    Score = Mathf.RoundToInt(Score + (300)*drainAmount + life);
                }
                else
                {
                    Score = Mathf.RoundToInt(Score + (300)*drainAmount);
                }

                ScoreBar.gameObject.GetComponent<Text>().text = Score.ToString();
                life = life + maxLife/7;
            }
            else {
                Debug.Log("Click, nothing.");
            }
        }
    }
    void setTime() {
        int mins = Mathf.FloorToInt(timeElapsed / 60);
        string seconds = Mathf.FloorToInt(timeElapsed - mins*60).ToString();
        if (seconds.Length==1)
        {
            seconds = "0" + seconds;
        }
        TimeBar.GetComponent<Text>().text = mins + ":" + seconds;
    }
    IEnumerator Counter() {
        while (!dead) {
            timeElapsed++;
            setTime();
            yield return new WaitForSeconds(1f);
        }
        
        
    }
    IEnumerator DrainLife() {
        float lifeBarScaleX = LifeBar.transform.localScale.x;
        while (life>0) {
            if ((drainAmount + (timeElapsed/6))/10>maxDrain) {
                life = life - maxDrain;
            }
            else {
                life = life - (drainAmount + (timeElapsed/6))/10;
            }
            float LifePercentage = life / maxLife; //returns something between 0f and 1f. Dependends on the player hp
            if (life < maxLife) {
                LifeBar.GetComponent<SpriteRenderer>().color = new Color(1, LifePercentage, LifePercentage);    
            }
            else {
                LifeBar.GetComponent<SpriteRenderer>().color = new Color(0.66f, 0.02f, 0.92f);
            }

            if (LifePercentage>1)
            {
                LifeBar.transform.localScale = new Vector3(1f, LifeBar.transform.localScale.y, -1);    
            }
            else
            {
                LifeBar.transform.localScale = new Vector3(LifePercentage, LifeBar.transform.localScale.y, -1);
            }
            
            yield return new WaitForSeconds(0.01f);    
        }
        LifeBar.GetComponent<Animation>().Play();
        SideBars[0].GetComponent<ParticleSystem>().Play();
        SideBars[1].GetComponent<ParticleSystem>().Play();
        audioSource.pitch = Random.Range(0.8f,1f);
        audioSource.Play();
        dead = true;
    }
    private void OnCollisionStay2D(Collision2D other) {
        collided = true;
        collidedObject = other.gameObject;
        
    }
    private void OnCollisionExit2D(Collision2D other) {
        collided = false;
        collidedObject = null;
    }
}
