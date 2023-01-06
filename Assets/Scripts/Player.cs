﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;

	private Animator animator;
	private int food;

	protected override void Start ()
	{
		animator = GetComponent<Animator>();
		
		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start();
	}

	// Called when this object is disabled
	private void OnDisable()
    {
		GameManager.instance.playerFoodPoints = food;
    }
	
	void Update () {
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		if (horizontal != 0)
			vertical = 0;

		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall>(horizontal, vertical); // passing the generic parameter <T> (type) Wall here, because we're expecting the player to interact with a wall.
	}

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove<T>(xDir, yDir);

		RaycastHit2D hit;

		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
    }

	private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.tag == "Exit")
		{ 
			Invoke("Restart", restartLevelDelay); // 1 sec pause then restart
			enabled = false;
		}
		else if (other.tag == "Food")
        {
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + "  Food: " + food;
			other.gameObject.SetActive(false);
        }
		else if (other.tag == "Soda")
        {
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + "  Soda: " + food;
			other.gameObject.SetActive(false);
        }
	}

    protected override void OnCantMove<T>(T component)
    {
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");
    }

	private void Restart()
    {
		SceneManager.LoadScene(0);
	}

	public void LoseFood (int loss)
    {
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
		if (food <= 0)
			GameManager.instance.GameOver();
	}
}
