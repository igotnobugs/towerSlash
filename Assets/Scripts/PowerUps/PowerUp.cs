using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Power ups attaches a colored Particle trail to enemies 
    Killing the enemy will activate the power up */

public enum PowerType { Frenzy, Shield, Dash, None }


public class PowerUp : MonoBehaviour 
{

    public PowerType powerType = PowerType.None;
    //protected float weightChance = 10.0f;
    // virtual void UsePowerUp() { }

    // float defaultDescentSpeed = .03f;
    //public float speedScale = 1.0f;
    //private ParticleSystem colorParticle = null;
    // float descentSpeed = 0.5f;

    /*
    private void Awake() {
        descentSpeed = defaultDescentSpeed;
    }

    private void Update() {
        transform.Translate(0, descentSpeed * -1.0f, 0);
    }

    public void ScaleDescentSpeed(float scale) {
        if (speedScale == scale) return;
        speedScale = scale;
        descentSpeed = defaultDescentSpeed * speedScale;
    }
    */
}
