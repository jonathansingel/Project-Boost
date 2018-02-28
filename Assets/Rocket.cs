using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsRotation = 100f;
    [SerializeField] float mainThrust = 1000f;

    Rigidbody rigidBody;
    AudioSource rocketThrusterSound;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rocketThrusterSound = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckThrust();
        CheckRotation();
    }

    private void CheckThrust()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            float thrustThisFrame = Time.deltaTime * mainThrust;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
                        
            if (rocketThrusterSound.isPlaying == false)
            {
                rocketThrusterSound.Play();
            }
        }
        else
        {
            if (rocketThrusterSound.isPlaying == true)
            {
                rocketThrusterSound.Stop();
            }
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
