﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(0, 0, 0);
    [SerializeField] float period = 2f;

    // remove from inspector later
    float movementFactor; // 0 for not moved, 1 for fully moved.

    Vector3 startingPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(period <= Mathf.Epsilon) { return; } //todo protect against period is zero
        float cycles = Time.time / period; //grows continually from zero

        const float tau = Mathf.PI * 2; // 6.28
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = (rawSinWave / 2f) + .5f;
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
	}
}
