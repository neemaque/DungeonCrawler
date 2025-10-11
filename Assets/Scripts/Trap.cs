
using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    private bool activated = false;
    public GameObject trapConsequencePrefab;
    void OnTriggerEnter2D(Collider2D other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !activated)
        {
            activated = true;
            Debug.Log("trap activated!");
            StartCoroutine(SpawnConsequence());
        }
    }
    IEnumerator SpawnConsequence()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(trapConsequencePrefab, new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
        gameObject.SetActive(false);
    }
}
