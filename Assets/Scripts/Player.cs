using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;


	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food : " + food;

        base.Start();
	}

    public void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }
	
	// Update is called once per frame
	void Update () {
        //If it's not the player's turn, exit the function.
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;     //Used to store the horizontal move direction.
        int vertical = 0;       //Used to store the vertical move direction.


        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            //Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
            //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // collision
        // check the tag
        if (other.tag == "Exit")
        {
            print("Hi");
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        } else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food : " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        } else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food : " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        } 

        else if (other.tag == "Spell")
        {
            animator.SetTrigger("playerHit");
            food--;
        }
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            //Call the GameOver function of GameManager.
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // lose food on move
        food--;
        foodText.text = "Food : " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move (xDir,yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        // change animation trigger
        animator.SetTrigger("playerChop");
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food : " + food;
        
        CheckIfGameOver();
    }

    private void Restart()
    {
        //print("restart");
       
        SceneManager.LoadScene(0);
    }
}