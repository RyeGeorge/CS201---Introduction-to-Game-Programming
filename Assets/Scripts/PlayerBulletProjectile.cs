using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerBulletProjectile : MonoBehaviour
{
    public float speed;
    public float damage;
    private PlayerMovement playerMovement;
    public Vector3 bulletDirection;

    private void Start()
    {
        transform.parent = null;
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        if (playerMovement.isFacingRight)
            bulletDirection *= 1;
        else
            bulletDirection *= -1;
    }

    private void Update()
    {
        transform.Translate(bulletDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }

}
