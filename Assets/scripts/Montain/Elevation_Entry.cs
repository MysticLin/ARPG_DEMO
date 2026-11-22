using UnityEngine;

public class Elevation_Entry : MonoBehaviour
{
    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;
    public int level = 1;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Player_Move player=collision.GetComponentInParent<Player_Move>();
        if(player!=null)
            player.SetElevation(level);
        foreach (Collider2D mountain in mountainColliders)
            {
                mountain.enabled = false;
            }
            foreach (Collider2D boundary in boundaryColliders)
            {
                boundary.enabled = true;
            }
       SpriteRenderer sr= collision.gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 15;
    }
}
