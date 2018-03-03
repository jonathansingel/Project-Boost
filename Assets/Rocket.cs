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

    bool isTransitioning = false;

    bool collisionsDisabled = false;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //stop sound on death
        if (!isTransitioning)
        {
            if (Debug.isDebugBuild)
            {
                CheckDebugging(); // todo only if debugging is on.
            }
            CheckThrust();
            CheckRotation();
        }
    }

    private void CheckDebugging()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadFirstLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isTransitioning || collisionsDisabled) { return; }

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
        isTransitioning = true;
        StopSoundIfAlreadyPlaying();
        audioSource.PlayOneShot(levelLoadSound);
        Invoke("LoadNextLevel", levelDelay);
    }

    private void StartDeathSequence()
    {
        isTransitioning = true;
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex;

        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex + 1 < totalScenes)
        {
            nextSceneIndex = currentSceneIndex + 1;
        }
        else
        {
            nextSceneIndex = 0;
        }
        
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void CheckThrust()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust()
    {
        StopSoundIfAlreadyPlaying();
        mainEngineParticleSystem.Stop();
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
        rigidBody.angularVelocity = Vector3.zero;

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
    }

}
