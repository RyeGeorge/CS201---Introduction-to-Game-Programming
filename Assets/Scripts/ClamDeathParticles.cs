using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClamDeathParticles : MonoBehaviour
{
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;

    public void StartDeathEffect()
    {
        InstantiateObject(obj1);
        InstantiateObject(obj2);
        InstantiateObject(obj3);
        InstantiateObject(obj4);
    }

    private void InstantiateObject(GameObject obj)
    {
        GameObject newobj = Instantiate(obj, obj.transform.position, obj.transform.rotation);
        newobj.transform.position = transform.position;

        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector2 randomDir = new Vector2(randomX, randomY);

        newobj.GetComponent<Rigidbody2D>().AddForce(randomDir * 3, ForceMode2D.Impulse);
        newobj.GetComponent<Rigidbody2D>().AddTorque(Random.Range(10, 25));

        newobj.transform.parent = null;
    }
}
