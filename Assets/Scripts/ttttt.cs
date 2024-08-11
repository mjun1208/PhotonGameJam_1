using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttttt : MonoBehaviour
{
    public Player Player;
    public FishingRodLine FishingRodLine;
    public ParticleSystem FeedFx;
    public AudioSource sound;
    
    public void GOFinalPosition()
    {
        FishingRodLine.GOFinalPosition();
    }
    
    public void GoFeeding()
    {
        FeedFx.gameObject.SetActive(true);
        FeedFx.Play();
    }
    
    public void GoShovel()
    {
        Player.DoDig();
    }
    
    public void HitOn()
    {
        Player.HitOn();
        sound.Play();
    }

    public void HitOff()
    {
        Player.HitOff();
    }
}
