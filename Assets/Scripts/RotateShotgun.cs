using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShotgun : MonoBehaviour
{
    private Transform player;

    private Vector3 mouse_pos;
    private Vector3 object_pos;
    private float angle;

    PlayerMovement playerMovement;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        mouse_pos = Input.mousePosition;
        mouse_pos.z = 5f; //The distance between the camera and object
        object_pos = Camera.main.WorldToScreenPoint(transform.position);
        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;

        if (playerMovement.isFacingRight)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));

        if (!CursorRight() && playerMovement.isFacingRight || CursorRight() && !playerMovement.isFacingRight)
        {
            FlipPlayer();
        }
    }

    void FlipPlayer()
    {
        Debug.Log("Flip");

        playerMovement.isFacingRight = !playerMovement.isFacingRight;
        Vector3 localScale = player.localScale;
        localScale.x *= -1f;
        player.localScale = localScale;
    }

    bool CursorRight()
    {
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x > (float)Screen.width / 2f;
    }
}
