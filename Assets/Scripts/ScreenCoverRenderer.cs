using System.Collections;
using UnityEngine;

public class ScreenCoverRenderer : MonoBehaviour
{
    public float FadeOutTime = 0.2f;
    public float FadeInTime = 0.2f;

    private Renderer[] _screenCoverRenders; // array that contains all of our screen covers
    private float _time; // keeps track of the fading of our covers
    private enum CoverState { FadingIn, FadingOut, None } // enum that we use to keep track of what state we're in
    private CoverState _gameState; // The current state we're in

    void Start()
    {
        _screenCoverRenders = GetComponentsInChildren<Renderer>(); // get all the renderer of our child game objects
        SetCoverAlpha(0);
        _gameState = CoverState.None;
    }

    // Returns true if we're in the middle of a coroutine animation. False otherwise.
    public bool IsAnimating()
    {
        return _gameState != CoverState.None;
    }

    // Starts the FadeOut coroutine if we're not already in the middle of a coroutine. Otherwise nothing happens.
    public void StartFadeOut()
    {
        if (_gameState == CoverState.None)
        {
            _gameState = CoverState.FadingOut;
            StartCoroutine(FadeOut());
        }
    }


    // Starts the FadeIn coroutine if we're not already in the middle of a coroutine. Otherwise nothing happens.
    public void StartFadeIn()
    {
        if (_gameState == CoverState.None)
        {
            _gameState = CoverState.FadingIn;
            StartCoroutine(FadeIn());
        }
    }

    // Coroutine to cover the player's screen
    private IEnumerator FadeIn()
    {
        _time = 0f;
        while (_time < FadeInTime)
        {
            Fade(0, 1, FadeInTime); // 1 is opaque, 0 is transparent
            yield return null; // wait until the next frame
        }
        _gameState = CoverState.None; // now that we're done, go back to the none state
    }

    // Coroutine to remove the cover from the the player's screen
    private IEnumerator FadeOut()
    {
        _time = 0f;
        while (_time < FadeOutTime)
        {
            Fade(1, 0, FadeOutTime); // 1 is opaque, 0 is transparent
            yield return null; // wait until the next frame
        }
        _gameState = CoverState.None; // now that we're done, go back to the none state
    }

    // Helper function to change the alpha of our screen cover
    private void Fade(float start, float end, float fadeTime)
    {
        _time += Time.deltaTime;
        float currentAlpha = Mathf.Lerp(start, end, _time / fadeTime);
        SetCoverAlpha(currentAlpha);
    }

    // Helper function to change the alpha of all the cover game objects. We have to 
    // change the material directly, we can't hold a reference to the color variable
    private void SetCoverAlpha(float alpha)
    {
        foreach (Renderer screenCoverRender in _screenCoverRenders)
        {
            Color color = screenCoverRender.material.color;
            color.a = alpha;
            screenCoverRender.material.color = color;
        }
    }
}
