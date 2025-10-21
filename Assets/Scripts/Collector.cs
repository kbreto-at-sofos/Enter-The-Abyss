using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        var item = collision.GetComponent<IItem>();

        item?.Collect();
    }
}
