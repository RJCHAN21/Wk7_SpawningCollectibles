using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform orbitingObject;
    [SerializeField] Transform centerObject;
    [SerializeField] float speed = 180.0f;
    private bool moveClockwise = true;

    void Update()
    {
        float rotationDirection = moveClockwise ? -1.0f : 1.0f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            moveClockwise = !moveClockwise;
        }

        orbitingObject.RotateAround(centerObject.position, Vector3.forward, rotationDirection * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            GameManager.Instance.AddScore(1);
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance?.TriggerGameOver();
        }
    }
}
