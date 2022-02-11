using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaControl : MonoBehaviour 
{
    public float attackRange = 1.5f;

    private List<Swipeable> targets = new List<Swipeable>();
    //private Swipeable currentTarget;

    private Animator animator;
    //private bool isAttacking = false;
    private float previousPosition;

    public bool isRhythmMode = false;
    public bool hasAttacked = false;

    private void Awake() {
        animator = GetComponent<Animator>();
    }


    public void PlayAnimation(string anim) {
        animator.Play(anim);
    }


    public void AddTarget(Swipeable targetToAdd) {
        targetToAdd.InRange += AttackTarget;
        
        targets.Add(targetToAdd);
    }

    private void AttackTarget(Swipeable swipeable) {
        swipeable.InRange -= AttackTarget;
        //isAttacking = true;
        //
        hasAttacked = true;

        animator.SetInteger("targets", animator.GetInteger("targets") + 1);
        if (animator.GetInteger("targets") <= 1) {
            animator.Play("SlashAnimation");
        } else {

        }

        targets.RemoveAt(0);

        Destroy(swipeable.gameObject);
        //isAttacking = false;
    }

    private void JustAttack(Enemy enemy) {
        //isAttacking = true;
        animator.Play("SlashAnimation");
        targets.RemoveAt(0);
        Destroy(enemy.gameObject);
        //isAttacking = false;
    }

    private void Attack(Enemy enemy) {
        //isAttacking = true;

        //successfulAttacks--;

        Hashtable hashTable = new Hashtable {
            { "time", 0.5f },
            { "oncomplete", "EndAttack" }
        };

        previousPosition = transform.position.x;
        
        switch (enemy.enemyType) {
            case EnemyType.Walking:
                animator.Play("SlashAnimation");
                Destroy(enemy.gameObject);
                break;
            case EnemyType.Flying:
                Vector3 targetPosition = enemy.transform.position;

                hashTable.Add("x", targetPosition.x);
                hashTable.Add("y", transform.position.y);
                hashTable.Add("z", transform.position.z);

                animator.Play("SpinAnimation");
                Destroy(enemy.gameObject);
                break;
            case EnemyType.TowerObstacle:
                hashTable.Add("x", transform.position.x - 5.0f);
                hashTable.Add("y", transform.position.y);
                hashTable.Add("z", transform.position.z);

                animator.Play("SpinAnimation");

                break;
        }

        iTween.MoveTo(gameObject, hashTable);
    }

    private void EndAttack() {
        targets.RemoveAt(0);

        Hashtable hashTable = new Hashtable {
            { "x", previousPosition },
            { "y", transform.position.y },
            { "z", transform.position.z },
            { "time", 0.4f },
            { "oncomplete", "ReadyForAttack" }
        };

        iTween.MoveTo(gameObject, hashTable);
    }

    private void ReadyForAttack() {
        //Enemy enemy = currentTarget.GetComponent<Enemy>();
        //GameManager.Instance.ActivatePowerUp(enemy.powerUp);

        //if (enemy.enemyType != EnemyType.TowerObstacle)
        //    Destroy(currentTarget.gameObject);

        //currentTarget = null;
        //isAttacking = false;
    }

    public void Reset() {
        if (hasAttacked) {
            animator.SetInteger("targets", 0);
            hasAttacked = false;
        }
    }
}
