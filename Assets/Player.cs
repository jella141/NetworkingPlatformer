using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{

    public float rayDistance = 1f;
    public float jumpHeight = 50;
    public float movementSpeed = 20f;
    public int health = 100;
    public int stamina = 50;
    public int armor = 300;
    public LayerMask layerMask;

    private bool canJump = true;
    private bool isJumping = false;
    private bool isSpaceDown = false;
    private Rigidbody2D rigid;
    private Vector3 movement;
    private SpriteRenderer renderer;
    private Camera cam;
    private Vector2 hitNormal;
    

    void Start()
    {
    
    cam = GetComponentInChildren<Camera>();
      
        rigid = GetComponent<Rigidbody2D>();
        //movement = transform.position;
        renderer = GetComponent<SpriteRenderer>();
        renderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        HandleNetwork();
    }

    void HandleNetwork()
    {
        if(!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
        
        Jump();
        Movement();       
        
        }
    }

    void FixedUpdate()
    {
        Bounds bounds = renderer.bounds;
        Vector3 size = bounds.size;
        Vector3 scale = transform.localScale;
        float height = (size.y *.5f) *scale.y;

        Vector3 origin = transform.position + Vector3.down * height;
        Vector3 direction = Vector3.down;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, ~layerMask.value);
        Debug.DrawLine(origin, origin + direction * rayDistance, Color.red);
        if(hit.collider != null)
        {
            isJumping = false;
        }
    }

    void Movement()
    {
        float inputHoriz = Input.GetAxis("Horizontal");

        rigid.velocity = new Vector2(0, rigid.velocity.y);

        if (inputHoriz > 0 && hitNormal.x >= 0)
        {
            rigid.velocity = new Vector2( movementSpeed, rigid.velocity.y);
        }

        if (inputHoriz < 0 && hitNormal.x <= 0)
        {
            rigid.velocity = new Vector2(-movementSpeed, rigid.velocity.y);
        }


    }

    void Jump()
    {
        if (!isJumping)
        {

            float inputVert = Input.GetAxis("Jump");
            if (inputVert > 0 && canJump)
            {
                canJump = false;
                rigid.AddForce(Vector3.up * inputVert * jumpHeight, ForceMode2D.Impulse);
                isJumping = true;
            }
            if (inputVert == 0);
            {
                canJump = true;
            }

        }
    }

    void OnCollisionStay2D(Collision2D Col)
    {
        hitNormal = Col.contacts[0].normal;
        Vector3 point = Col.contacts[0].point;
        Debug.DrawLine(point, point + (Vector3)hitNormal * 5, Color.red);
    }

   void OnCollisionExit2D(Collision2D col)
    {
        hitNormal = Vector2.zero;
    }


   
}
