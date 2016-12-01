using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public AudioSource Win;
    public AudioSource Loss;
    public AudioSource Highlight;
    public AudioSource Select;
    public AudioSource Move;

    public void PlayWin()
    {
        Win.Play();
    }

    public void PlayLoss()
    {
        Loss.Play();
    }

    public void PlayHighlight()
    {
        Highlight.Play();
    }

    public void PlaySelect()
    {
        Select.Play();
    }

    public void PlayMove()
    {
        Move.Play();
    }
}
