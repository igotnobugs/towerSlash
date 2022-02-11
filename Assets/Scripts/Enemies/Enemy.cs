using UnityEngine;
// Standard monsters

// Usually health and other common functions

public enum EnemyType { Walking, Flying, TowerObstacle };

public class Enemy : Swipeable 
{ 
    public EnemyType enemyType;
    public PowerType powerUp = PowerType.None;

    public float defaultDescentSpeed = 8.0f;
    public float speedScale = 1.0f;

    protected float descentSpeed = 8.0f;

    protected virtual void DoKillAnimation() {}
    protected virtual void DoDeathAnimation() {}

    private void Awake() {
        descentSpeed = defaultDescentSpeed;
    }

    private void Start() {
        StartArrowPhase();
        GameManager.Instance.OnUpdateSpeedScale += ScaleDescentSpeed;
    }

    private void Update() {
        transform.Translate(0, descentSpeed * -1.0f * Time.deltaTime, 0);
        Destroy(gameObject, 15.0f);
    }

    public void ScaleDescentSpeed(float scale) {
        if (speedScale == scale) return;
        speedScale = scale;
        descentSpeed = defaultDescentSpeed * speedScale;
    }

}
