using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerBulletProjectile : MonoBehaviour
{
    public float speed;
    private PlayerMovement playerMovement;
    private Vector3 bulletDirection;

    private void Start()
    {
        transform.parent = null;
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        if (playerMovement.isFacingRight)
            bulletDirection = Vector3.right;
        else
            bulletDirection = Vector3.left;
    }

    private void Update()
    {
        transform.Translate(bulletDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
