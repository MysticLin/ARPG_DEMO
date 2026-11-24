using System.Collections;
using System.ComponentModel;
using System.Net.NetworkInformation;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float lifetime = 0.6f;
    [SerializeField] private float moveY = 1.2f;
    public void Show(int damage,Color color)
    {
        text.color = color;
        text.text=damage.ToString();
        StartCoroutine(Play());
    }
    private IEnumerator Play()
    {
        Vector3 start = transform.position;
        Vector3 end=start+Vector3.up*moveY;
        float t = 0f;
        while (t<lifetime)
        {
            t += Time.deltaTime;
            float p = t / lifetime;
            transform.position=Vector3.Lerp(start,end,p);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f - p);
            yield return null;
        }
        Destroy(gameObject);
    }
}

