
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerControll : MonoBehaviour {

	//A LOT OF VARIABLES ARE SET TO PUBLIC, BUT YOU CAN SET TO PRIVATE
	//I ONLY DID THIS SO I CAN SEE IN THE ENGINE


	//PHYSICS
	public Rigidbody2D				 PlayerRigidbody;
	public int 						forcejump;
	public Animator 				 anime;

	//Verifica se o player esta tocando no chao
	public Transform				 GroundCheck;
	public bool 					grounded;
	public LayerMask 				WhatIsGround;

	//GAMEOVER
	public static bool gameOver;

	//TOUCH
	public bool touch;
	public bool touchUp;
	public bool nottouch;



	//Pulo / JUMP
	public bool                      jump;
    //public bool gameoverEND;


	//Times

	public float 					jumpTempo;
	public float 					slideTempo;
	private float 					timeTempo;   //jump
	private float                    timeTempo2; //slide

	// VIDA / LIFE
	public int					  	    maxvida = 3;
	public static int 		     		quantvida;

	//MOEDAS / COINS NEVER USED
	public static int       moeda;



	//SWIPE
	private float fingerStartTime  = 0.0f;
	private Vector2 fingerStartPos = Vector2.zero;
	public bool isSwipe = false;
	private float minSwipeDist  = 50.0f;
	private float maxSwipeTime = 0.5f;

	//colisor do slide / COLISOR SLIDE
	public Transform                colisor;

	//Audio
	public AudioSource    audioFont;
	public AudioClip      soundJump;


	// Use this for initialization
	void Start () {

		gameOver = false;
		quantvida = 0;
        //gameoverEND = false;


    }
	
	// Update is called once per frame
	void Update () {


//---------------------------------------------------------------------------------------------------------------------------------
		//Here's I sumulate the touch
		if (Input.GetKeyDown(KeyCode.Space)) {
            if (grounded == true && jump == false)
            {

                //if the character is slide
                //se o personagem estiver fazendo slide


                //make the player jump
                //faz player pular.
                PlayerRigidbody.AddForce(new Vector2(0, forcejump));
				
				
				//volume
				//volume
				//volume
				//if (audioFont)
				//	audioFont.volume = 0.3f;
				//play the song
				//Toca o audio
				if (soundJump != null && audioFont != null)
					audioFont.PlayOneShot(soundJump);

				//jump = true;
			}

        }

	//Here the scrip create a circle and verify if the character is touching the ground, 0.2f is the size of the circle.
		// aqui ele cria um pequeno circulo e verifica se esta encostando no chao, 0.2f e o tamanho do circulo
		grounded = Physics2D.OverlapCircle (GroundCheck.position, 0.1f, WhatIsGround);
		
		
		//Makes the "Jump" from the Animatior be activate when the character is NOT touching the ground
		//faz com que o "jump" do animator seja ativado quando o personagem NAO estiver tocando no chao.
		
		anime.SetBool ("jump", !grounded);
		
		//anime.SetBool ("gameover", gameOver);

//------------------------- MOUSE CONTROLLS ------------------------------------------------------------------------------------------------------------------

	}
	
	

	//iff the character touch another collider 2d
	void OnTriggerEnter2D(Collider2D other)
	{

	
	}
    public void gameoverEND ()
    {
        //SceneManager.LoadScene("Game Over");


    }







}