using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isPlayerHurt = false;

    private Animator animator;
    private Vector2 lastDirection = new Vector2(0, -1);

    private int cropsCollected = 0;

    public float recoilForce = 3f;
    public float recoilDuration = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
    }

    void Update()
    {
        if (isPlayerHurt)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        bool isMoving = movement.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            Vector2 moveDir = movement.normalized;
            animator.SetFloat("moveX", moveDir.x);
            animator.SetFloat("moveY", moveDir.y);
            lastDirection = moveDir;
        }
        else
        {
            animator.SetFloat("moveX", lastDirection.x);
            animator.SetFloat("moveY", lastDirection.y);
        }
    }

    void FixedUpdate()
    {
        if (!isPlayerHurt)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") && !isPlayerHurt)
        {
            Debug.Log("Player is hurt");
            StartCoroutine(ApplyRecoil(collision.transform));
        }
    }

    private IEnumerator ApplyRecoil(Transform monsterTransform)
    {
        isPlayerHurt = true;
        animator.SetTrigger("hurt");

        Vector2 recoilDirection = (transform.position - monsterTransform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(recoilDuration);

        isPlayerHurt = false;
        rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crop"))
        {
            cropsCollected++;
            Debug.Log("Crop harvested: " + cropsCollected);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Animal"))
        {
            AnimalSound animal = other.GetComponent<AnimalSound>();
            if (animal != null)
            {
                Debug.Log(animal.sound);
            }
            else
            {
                Debug.Log("Animal sound!");
            }
        }
    }
}