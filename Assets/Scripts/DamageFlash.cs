using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    public float flashTime = 0.2f;

    private SpriteRenderer[] spriteRenderers;
    private Material[] materials;

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        //Assign materials
        materials = new Material[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            materials[i] = spriteRenderers[i].material;
        }
    }

    public void TriggerFlash()
    {
        StartCoroutine(Flash());
    }


    IEnumerator Flash()
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_FlashAmount", 1);
        }

        yield return new WaitForSeconds(flashTime);

        foreach (Material mat in materials)
        {
            mat.SetFloat("_FlashAmount", 0);
        }
    }

}
