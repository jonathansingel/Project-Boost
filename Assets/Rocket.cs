using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsRotation = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float levelDelay = 2f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelLoadSound;

    [SerializeField] ParticleSystem mainEngineParticleSystem;
    [SerializeField] ParticleSystem explosionParticleSystem;
    [SerializeField] ParticleSystem levelCompleteParticleSystem;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

	// Use this for initialization
	void Start () {
        state = State.Alive;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //stop sound on death
        if (state == State.Alive)
        {
            CheckThrust();
            CheckRotation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive){ return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                levelCompleteParticleSystem.Play();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        StopSoundIfAlreadyPlaying();
        audioSource.PlayOneShot(levelLoadSound);
        Invoke("LoadNextLevel", levelDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        StopSoundIfAlreadyPlaying();
        audioSource.PlayOneShot(deathSound);
        mainEngineParticleSystem.Stop();
        explosionParticleSystem.Play();
        Invoke("LoadFirstLevel", levelDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void CheckThrust()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            StopSoundIfAlreadyPlaying();
            mainEngineParticleSystem.Stop();
        }
    }

    private void StopSoundIfAlreadyPlaying()
    {
        if (audioSource.isPlaying == true)
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        mainEngineParticleSystem.Play();

        if (audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(mainEngineSound);
        }

    }

    private void CheckRotation()
    {
        rigidBody.freezeRotation = true;

        float rotationThisFrame = Time.deltaTime * rcsRotation;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

}
