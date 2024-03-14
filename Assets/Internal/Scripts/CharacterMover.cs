using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _animator.SetTrigger("PullRope");
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.parent.position = transform.parent.position + transform.parent.forward * 10 * Time.deltaTime;
            _animator.SetTrigger("WalkForward");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.parent.position = transform.parent.position + transform.parent.forward * -10 * Time.deltaTime;
            _animator.SetTrigger("WalkBack");
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.parent.Rotate(new Vector3(0, Time.deltaTime * -100, 0));
            _animator.SetTrigger("WalkLeft");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.parent.Rotate(new Vector3(0, Time.deltaTime * 100, 0));
            _animator.SetTrigger("WalkRight");
        }
        else
        {
            _animator.SetTrigger("Stop");
        }
    }
}

