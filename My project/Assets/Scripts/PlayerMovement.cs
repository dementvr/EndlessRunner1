using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 2f;       // движение вперёд по Z
    public float horizontalSpeed = 3f;   // движение по X
    public float leftLimit = -5.5f;
    public float rightLimit = 5.5f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;         // начальная скорость прыжка
    public float gravity = -9.81f;       // ускорение свободного падения

    private float groundY;               // уровень «земли»
    private float yVelocity = 0f;        // текущая вертикальная скорость
    private bool isJumping = false;      // в прыжке ли мы?
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        groundY = transform.position.y;  // запомним стартовую высоту
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // ——————— движение вперёд/влево/вправо ———————
        transform.Translate(Vector3.forward * playerSpeed * dt, Space.World);

        if (Input.GetKey(KeyCode.A) && transform.position.x > leftLimit)
            transform.Translate(Vector3.left * horizontalSpeed * dt, Space.World);

        if (Input.GetKey(KeyCode.D) && transform.position.x < rightLimit)
            transform.Translate(Vector3.right * horizontalSpeed * dt, Space.World);

        // ——————— прыжок ———————
        // по событию нажатия клавиши и только если не в прыжке
        if ((Input.GetKeyDown(KeyCode.Space) ||
             Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.UpArrow)) && !isJumping)
        {
            isJumping = true;
            yVelocity = jumpForce;
            if (animator) animator.SetTrigger("Jump");
        }

        if (isJumping)
        {
            // гравитация
            yVelocity += gravity * dt;
            // перемещаем по Y
            transform.Translate(Vector3.up * yVelocity * dt, Space.World);

            // если опустились ниже или на землю — останавливаем прыжок
            if (transform.position.y <= groundY)
            {
                Vector3 pos = transform.position;
                pos.y = groundY;
                transform.position = pos;

                isJumping = false;
                yVelocity = 0f;

                if (animator) animator.SetTrigger("Land");
            }
        }
    }
}
