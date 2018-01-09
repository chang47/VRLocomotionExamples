using System;
using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject ScreenCover; // the object that's covering our screen
    public TrajectorySimulator TrajectorySimulator; // Calculator for where our pointer line is
    public GameObject Controller;

    private Renderer _coverRenderer; // our material that contains our color
    private float _fadeInTime; // time to fade in our screen cover
    private float _fadeOutTime; // time to fade out our screen cover
    private float _timeInDark; // time we wait before we fade back in
    private float _time; // keep track of time
    private Boolean _isFading; // track if we are currently fading
    private Renderer _controllerRenderer;

	void Start () {
	    _coverRenderer = ScreenCover.GetComponent<Renderer>();
	    _controllerRenderer = Controller.GetComponent<Renderer>();
	    _fadeInTime = 0.2f;
	    _fadeOutTime = 0.2f;
	    _timeInDark = 0.4f;
	    _time = 0f;
	    _isFading = false;

        // set our starting color to be be 0 or transparent
	    SetCoverAlpha(0f);
	}
	
	void Update ()
	{
        // Only shoot when we're not in the middle of teleporting
	    if (Input.GetButtonDown("Fire1") && !_isFading)
	    {
            Shoot();
	    }
	}

    private void Shoot()
    {
        // shoot a raycast from the center of our screen
        //Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //RaycastHit hit; // output variable to get what we collided against
        if (TrajectorySimulator.hitObject != null)
        {
            // we hit something, cover our screen and teleport to the location
            StartCoroutine(FadeIn());
        }
    }

    // Coroutine to cover the player's screen
    private IEnumerator FadeIn()
    {
        _isFading = true;
        _time = 0f;
        while (_time < _fadeInTime)
        {    
            Fade(0, 1, _fadeInTime); // 1 is opaque, 0 is transparent
            yield return null; // wait until the next frame
        }

        // now that the screen is covered, set our location to the point we hit
        Vector3 hit = TrajectorySimulator.HitVector;
        Vector3 newLocation = new Vector3(hit.x, 1, hit.z);
        transform.position = newLocation;

        yield return new WaitForSeconds(_timeInDark); // wait in the dark
        StartCoroutine(FadeOut()); // start fading away the cover
    }

    // Coroutine to remove the cover from the the player's screen
    private IEnumerator FadeOut()
    {
        _time = 0f;
        while (_time < _fadeOutTime)
        {
            Fade(1, 0, _fadeOutTime); // 1 is opaque, 0 is transparent
            yield return null; // wait until the next frame
        }
        _isFading = false;
    }

    // Helper function to change the alpha of our screen cover
    private void Fade(float start, float end, float fadeTime)
    {
        _time += Time.deltaTime;
        float currentAlpha = Mathf.Lerp(start, end, _time / fadeTime);
        SetCoverAlpha(currentAlpha);
    }

    // Helper function to change the alpha of our cover material. We have to 
    // change the material directly, we can't hold a reference to the color variable
    private void SetCoverAlpha(float alpha)
    {
        Color color = _coverRenderer.material.color;
        color.a = alpha;
        _coverRenderer.material.color = color;
    }
}
