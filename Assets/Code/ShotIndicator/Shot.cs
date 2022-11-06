using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        var col = collision.gameObject.GetComponent<PlayerController>();

        if (col != null)
        {
            col.Kill();
        }
    }
}
