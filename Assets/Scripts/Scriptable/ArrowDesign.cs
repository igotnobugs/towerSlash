using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arrow Design")]
public class ArrowDesign : ScriptableObject 
{
    public GameObject arrowObject;
    public Sprite arrowSprite = null;
    public Sprite dotSprite = null;

    public ParticleSystem particle;
    public Color colorIsLiteral;
    public Color colorIsOpposite;
    public Color colorIsCorrect;
    public Color colorIsWrong;

}
