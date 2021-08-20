using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaControl : MonoBehaviour 
{
    public float attackRange = 1.5f;

    private List<GameObject> targets = new List<GameObject>();
    private GameObject currentTarget;

    private int successfulAttacks = 0;
    private Animator animator;
    private bool isAttacking = false;
    private float previousPosition;
    private GameManager gameManager;

    public bool isRhythmMode = false;

    private void Start()
	{
        animator = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void Update() 
	{
        if (isAttacking) return;

        if (currentTarget == null)
            currentTarget = GetTarget();

        if (successfulAttacks <= 0) return;

        if (currentTarget.transform.position.y < transform.position.y + attackRange) {
            Enemy enemy = currentTarget.GetComponent<Enemy>();

            if (isRhythmMode) {
                JustAttack(enemy);
            }
            else {
                Attack(enemy);
            }
        }
    }

    public void PlayAnimation(string anim) {
        animator.Play(anim);
    }

    private GameObject GetTarget() {
        if (targets.Count <= 0) return null;
        if (targets[0] == null) 
            targets.RemoveAll(item => item == null);
        return targets[0];
    }

    public void AddTarget(GameObject targetToAdd) {
        successfulAttacks++;
        targets.Add(targetToAdd);
    }

    private void JustAttack(Enemy enemy) {
        isAttacking = true;
        successfulAttacks--;
        animator.Play("SlashAnimation");
        targets.RemoveAt(0);
        Destroy(currentTarget);
        currentTarget = null;
        isAttacking = false;
    }

    private void Attack(Enemy enemy) {
        isAttacking = true;

        successfulAttacks--;

        Hashtable hashTable = new Hashtable {
            { "time", 0.5f },
            { "oncomplete", "EndAttack" }
        };

        previousPosition = transform.position.x;
        
        switch (enemy.enemyType) {
            case EnemyType.Walking:
                animator.Play("SlashAnimation");
                break;
            case EnemyType.Flying:
                Vector3 targetPosition = enemy.transform.position;

                hashTable.Add("x", targetPosition.x);
                hashTable.Add("y", transform.position.y);
                hashTable.Add("z", transform.position.z);

                animator.Play("SpinAnimation");
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
        Enemy enemy = currentTarget.GetComponent<Enemy>();
        gameManager.ActivatePowerUp(enemy.powerUp);

        if (enemy.enemyType != EnemyType.TowerObstacle)
            Destroy(currentTarget);

        currentTarget = null;
        isAttacking = false;
    }

    public bool IsAttacking() {
        return isAttacking;
    }
}
