using UnityEngine;

namespace ClearSky
{
    public class PlayerMovementController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 20f; //Set Gravity Scale in Rigidbody2D Component to 5

        private Rigidbody2D rb;
        private Animator anim;
        Vector3 movement;
        bool isJumping = false;
        private bool alive = true;
        private float jumpCount = 2;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        public float maxHealth = 100.0f;
        public float currentHealth;
        public float jumpDamageRate = 0.05f;
        public float runDamageRate = 0.1f;

        public HealthBar healthBar;


        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }

        private void Update()
        {
            if (alive)
            {
                Jump();
                Run();
            }
        }

        private bool IsGrounded()
        {
            return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        void Run()
        {
            Vector3 moveVelocity = Vector3.zero;
            anim.SetBool("isRun", false);
            Vector3 localScale = transform.localScale;

            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                moveVelocity = Vector3.left;
                
                if(localScale.x > 0) {
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }
                
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                moveVelocity = Vector3.right;
                 if(localScale.x < 0) {
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);

            }
            Vector3 positionChange = moveVelocity * movePower * Time.deltaTime;
            transform.position += moveVelocity * movePower * Time.deltaTime;
            TakeDamage(Mathf.Abs(positionChange.x) * runDamageRate);
        }
        void Jump()
        {
            if(IsGrounded())
            {
                jumpCount = 2;
            }
            anim.SetBool("isJump", !IsGrounded());
            if ((Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0)
            && jumpCount > 0)
            {  
                isJumping = true;
                jumpCount--;
            }
            if (!isJumping)
            {
                return;
            }

            rb.velocity = Vector2.zero;

            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

            TakeDamage(jumpDamageRate * maxHealth);

            isJumping = false;
        }

        void TakeDamage(float damage) {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }
    }
}