using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    public Text notificationText;
    public Text topBar;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;
    private int hp;
    private int mp;
    private int maxHp = 100;
    private int maxMp = 50;


	// Use this for initialization
	protected override void Start ()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;
        hp = GameManager.instance.playerHp;
        mp = GameManager.instance.playerMp;

        notificationText.text = "";
        topBar.text = "\t Hp : " + hp + "/100 \t Mp : " + mp + "/50";

        base.Start();
	}

    public void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.playerHp = hp;
        GameManager.instance.playerMp = mp;
    }

    internal void LoseHp(int loss)
    {
        animator.SetTrigger("playerHit");
        hp -= loss;
        notificationText.text = "Vous perdez " + loss + "hp.";

        CheckIfGameOver();
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

            hp += pointsPerFood;
            notificationText.text = "+" + pointsPerFood + " hp";
            if (hp > maxHp)
                hp = maxHp;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        } else if (other.tag == "Soda")
        {
            hp += pointsPerSoda;
            if (hp > maxHp)
                hp = maxHp;
            notificationText.text = "+" + pointsPerSoda + " hp";
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
<<<<<<< HEAD
        } 

        else if (other.tag == "Spell")
        {
            animator.SetTrigger("playerHit");
            food--;
        }
=======
        }

        UpdateInfos();
>>>>>>> 84f95e8004536d7acb42048ecdad3c858f64f890
    }

    private void CheckIfGameOver()
    {
        if (hp <= 0)
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
        /*food--;
        notificationText.text = "Food : " + food;*/
        notificationText.text = "";

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        bool cantMove = Move(xDir, yDir, out hit);

        if (hit.transform != null)
        {
            T hitComponent = hit.transform.GetComponent<T>();
            //print(hit.transform);
            //print(hitComponent)

            // we try if it's an enemy
            if (!cantMove && hitComponent == null && hit.transform.GetComponent<Enemy>() != null)
                OnCantMove(hit.transform.GetComponent<Enemy>());
            else if (!cantMove && hitComponent != null)
                OnCantMove(hitComponent);                            
        }

        
        RaycastHit2D hit2;
        if (Move (xDir,yDir, out hit2))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        UpdateInfos();
        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    public void UpdateInfos()
    {
        topBar.text = "\t Hp : " + hp + "/100 \t Mp : " + mp + "/50";
    }

    protected override void OnCantMove<T>(T component)
    {
        if (component.tag == "Wall")
        {
            Wall hitWall = component as Wall;
            hitWall.DamageWall(wallDamage);
        } else if (component.tag == "Enemy")
        {
            Enemy enemy = component as Enemy;
            enemy.DamageEnemy(1);
        }

        // change animation trigger
        animator.SetTrigger("playerChop");
    }

    /*public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food : " + food;
        
        CheckIfGameOver();
    }*/

    private void Restart()
    {
        //print("restart");
       
        SceneManager.LoadScene(0);
    }
}