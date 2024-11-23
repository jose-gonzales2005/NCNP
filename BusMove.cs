using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusMove : MonoBehaviour
{
    private Rigidbody rb;
    //public float diveForce = 20f;

    [Header("Move Stuff")]
    public float glideModifier = 0.5f;
    public float diveGravityMultiplier = 5f; // Gravity multiplier during dive
    public float normalGravityMultiplier = 2f; // Gravity multiplier during normal flight
    public float boostForce = 5f;
    public float moveSpeed = 8f;
    
    public float playerHeight;

    public float forwardSpeed = 5f;
    //public KeyCode 

    private bool isGrounded = false;
    private bool isDiving = false;

    public Transform orientation;

    [Header("slope handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeSmack;

    private Vector3 lastGroundNormal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(forwardSpeed, rb.velocity.y, rb.velocity.z);
        //rb.useGravity = false;        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            isDiving = true;
        }
        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space))
        {
            isDiving = false;
        }
        Debug.Log(OnSlope());
        if (OnSlope())
        {
            Debug.Log("adding slope froce");
            rb.AddForce(GetSlopeMoveDir() * moveSpeed * 20f, ForceMode.Force);
        }

        Debug.DrawRay(orientation.position, GetSlopeMoveDir() * 10, Color.green);
    }

    private void Dive()
    {
        //if (isGrounded)
        //rb.AddForce(Vector3.down * diveForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        //rb.velocity = new Vector3(forwardSpeed, rb.velocity.y, rb.velocity.z);

        if (!isGrounded)
        {
            ApplyGravy();
        }
    }

    /*private void SlideAlongTerrain()
    {
        Vector3 slopeDirection = Vector3.Cross(lastGroundNormal, Vector3.up);
        rb.AddForce(slopeDirection.normalized * boostForce, ForceMode.Acceleration);

        // Reduce velocity for smooth gliding on hills
        rb.velocity = new Vector3(rb.velocity.x * glideModifier, rb.velocity.y, rb.velocity.z * glideModifier);
    }*/

    private void ApplyGravy()
    {
        float gravity = isDiving ? diveGravityMultiplier : normalGravityMultiplier;
        rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            //stores normal of collision point 
            lastGroundNormal = collision.contacts[0].normal;

            //should reset dive state on landing
            isDiving = false;

            //hopefully makes landings smoother
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.3f));
        if (Physics.Raycast(transform.position, Vector3.down, out slopeSmack, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeSmack.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(orientation.forward, slopeSmack.normal).normalized;
    }
}