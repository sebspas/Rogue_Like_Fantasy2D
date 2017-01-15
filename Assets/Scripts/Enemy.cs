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
}