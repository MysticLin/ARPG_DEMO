using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Elevation_Exit : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;
    public int level = 0;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Player_Move player=collision.GetComponentInParent<Player_Move>();
        if(player!=null)
            player.SetElevation(level);
            foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = true;
            }
            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = false;
            }
        SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
        if(sr!=null)
            sr.sortingOrder = 10;
    }
}
