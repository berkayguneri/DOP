using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Animator anim;
    bool isRunning;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        // Oyuncunun giri�ini al
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Oyuncu hareket vekt�r�n� olu�tur
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Hareket vekt�r�n� d�nya koordinatlar�na d�n��t�r
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0f; // Y eksenindeki hareketi s�f�rla

        // Oyuncuyu hareket ettir
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Oyuncunun rotasyonunu ayarla
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        isRunning = movement.magnitude > 0;
        anim.SetBool("run", isRunning);
    }
}
