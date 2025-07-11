using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotateRotators : MonoBehaviour
{
    private Camera cam;
    private Vector3 screenPos;
    private float angleOffset;
    private Collider2D col;

    public Sinewave sinewave;
    public Sinewave desiredsine;
    private float desiredAmpl;
    private float desiredFreq;

    private float minAmpl;
    private float maxAmpl;
    private float minFreq;
    private float maxFreq;

    private float minPitch;
    private float maxPitch;
    private float minRev;
    private float maxRev;

    public bool isAmpl;
    private float progress;

    private AudioSource waveSound;

    public GameObject hintStart;
    public GameObject hintAmpl;
    public GameObject hintFreq;
    public GameObject hintSignal;

    public HintsAppear hintsAppear;

    private void Start()
    {
        cam = Camera.main;
        col = GetComponent<Collider2D>();
        desiredAmpl = desiredsine.amplitude;
        desiredFreq = desiredsine.frequency;
        minAmpl = 0.2f * sinewave.amplitude;
        maxAmpl = 5f * sinewave.amplitude;
        minFreq = 0.5f * sinewave.frequency;
        maxFreq = 2f * sinewave.frequency;

        waveSound = sinewave.GetComponent<AudioSource>();
        minPitch = waveSound.pitch * 0.5f;
        maxPitch = waveSound.pitch * 2f;
        minRev = waveSound.reverbZoneMix * 0.5f;
        maxRev = waveSound.reverbZoneMix * 2f;

        StartCoroutine(HintStart());
    }

    private void Update()
    {       
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (col == Physics2D.OverlapPoint(mousePos))
            {
                screenPos = cam.WorldToScreenPoint(transform.position);
                Vector3 vec3 = Input.mousePosition - screenPos;
                angleOffset = (Mathf.Atan2(transform.right.y, transform.right.x) - Mathf.Atan2(vec3.y, vec3.x)) * Mathf.Rad2Deg;

                waveSound.Play();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (col == Physics2D.OverlapPoint(mousePos))
            {
                Vector3 vec3 = Input.mousePosition - screenPos;
                float angle = Mathf.Atan2(vec3.y, vec3.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, angle + angleOffset);
                progress = transform.eulerAngles.z / 360f;

                if (isAmpl == true)
                {
                    sinewave.amplitude = Mathf.Lerp(maxAmpl, minAmpl, progress);
                    waveSound.pitch = Mathf.Lerp(maxPitch, minPitch, progress);

                    if (hintsAppear.seenAmpl == false)
                    {
                        StartCoroutine(HintAmpl());
                        hintsAppear.seenAmpl = true;
                    }
                }
                else
                {
                    sinewave.frequency = Mathf.Lerp(maxFreq, minFreq, progress);
                    waveSound.reverbZoneMix = Mathf.Lerp(maxRev, minRev, progress);

                    if (hintsAppear.seenFreq == false)
                    {
                        StartCoroutine(HintFreq());
                        hintsAppear.seenFreq = true;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
            {
                waveSound.Stop();
            }

        if (sinewave.amplitude > (desiredAmpl * 0.95f) && sinewave.amplitude < (desiredAmpl * 1.05f) && sinewave.frequency > (desiredFreq * 0.95f) && sinewave.frequency < (desiredFreq * 1.05f))
        {
            SceneManager.LoadScene("VictoryScreen");
        }

        if (hintsAppear.seenFreq == true && hintsAppear.seenAmpl == true && hintsAppear.seenSignal == false)
        {
            StartCoroutine(HintSignal());
            hintsAppear.seenSignal = true;
        }

    }

    IEnumerator HintStart()
    {
        yield return new WaitForSeconds(3);

        hintStart.SetActive(true);

        yield return new WaitForSeconds(5);

        hintStart.SetActive(false);
    }

    IEnumerator HintAmpl()
    {
        hintAmpl.SetActive(true);

        yield return new WaitForSeconds(5);

        hintAmpl.SetActive(false);
    }

    IEnumerator HintFreq()
    {
        hintFreq.SetActive(true);

        yield return new WaitForSeconds(5);

        hintFreq.SetActive(false);
    }

    IEnumerator HintSignal()
    {
        yield return new WaitForSeconds(7);

        hintSignal.SetActive(true);

        yield return new WaitForSeconds(5);

        hintSignal.SetActive(false);
    }
}
