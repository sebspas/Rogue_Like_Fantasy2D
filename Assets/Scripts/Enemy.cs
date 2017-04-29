using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    public int hp = 3;

    // Use this for initialization
    protected override void Start()
    {
        // add enemy script itself to list
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("EnemyAttack");

        //hitPlayer.LoseFood(playerDamage);
        hitPlayer.LoseHp(playerDamage);
        hitPlayer.UpdateInfos();

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        // dont move on the player turn
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        // smae column
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;

        } else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("OnTriggerEnter2D access");
        // collision
        // check the tag
        if (other.tag == "Trap")
        {
            DamageEnemy(1);
        }
    }

    public void DamageEnemy(int loss)
    {
        print("Damage Enemy " + loss);
        hp -= loss;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
            GameManager.instance.RemoveEnemies(this);            
        }
    }

    // to change sprite anim depending on direction of pnj
    // Update is called once per frame
    void Update()
    {

        var vertical = Input.GetAxis("Vertical");
        var horizontal = Input.GetAxis("Horizontal");

        if (vertical > 0)
        {
            animator.SetFloat("vertical", 1.0f);
            animator.SetFloat("horizontal", 0.0f);
        }
        else if (vertical < 0)
        {
            animator.SetFloat("vertical", -1.0f);
            animator.SetFloat("horizontal", 0.0f);
        }
        else if (horizontal < 0)
        {
            animator.SetFloat("vertical", 0.0f);
            animator.SetFloat("horizontal", -1.0f);
        }
        else if (horizontal > 0)
        {
            animator.SetFloat("vertical", 0.0f);
            animator.SetFloat("horizontal", 1.0f);
        }
        else
        {
            animator.SetFloat("vertical", 0.0f);
            animator.SetFloat("horizontal", 0.0f);
        }
    }
}