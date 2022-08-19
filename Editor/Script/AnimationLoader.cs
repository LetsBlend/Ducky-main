using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ScriptableObjects", menuName = "ScriptableObjects/AnimationData")]
public class AnimationLoader : ScriptableObject
{
    public Texture2D[] Sleeping;
    public Texture2D[] Falling;
    public Texture2D[] Idleling1;
    public Texture2D[] Idleling2;
    public Texture2D[] Shaking;
    public Texture2D[] Quack;

    private void OnEnable()
    {
        Sleeping = new Texture2D[81];
        for (int i = 0; i < 81; i++)
        {
            Sleeping[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/sleeping/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
        Falling = new Texture2D[7];
        for (int i = 0; i < 7; i++)
        {
            Falling[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/falling/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
        Idleling1 = new Texture2D[81];
        for (int i = 0; i < Idleling1.Length; i++)
        {
            Idleling1[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/Idle1/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
        Idleling2 = new Texture2D[81];
        for (int i = 0; i < Idleling2.Length; i++)
        {
            Idleling2[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/Idle2/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
        Shaking = new Texture2D[18];
        for (int i = 0; i < Shaking.Length; i++)
        {
            Shaking[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/Shaking/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
        Quack = new Texture2D[12];
        for (int i = 0; i < Quack.Length; i++)
        {
            Quack[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Packages/com.letsblend.ducky/Editor/Animations/Quack/00" + (i < 10 ? "0" + i : i) + ".png", typeof(Texture2D));
        }
    }
}
