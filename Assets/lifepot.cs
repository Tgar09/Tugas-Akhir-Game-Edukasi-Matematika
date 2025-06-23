using System.Collections;
using UnityEngine;

public class lifepot : MonoBehaviour
{
    [Header("Config")]
    public float moveSpeed = 5f;
    public float deactivateDelay = 0.2f; // ganti nama dari destroyDelay
    public float healthRestoreAmount = 20f;
    public float manaRestoreAmount = 20f;

    [Header("Item")]
    public InventoryItem itemToAdd;
    public int quantity = 1;

    private bool isAbsorbed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAbsorbed)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            PlayerMana playerMana = other.GetComponent<PlayerMana>();

            if (playerHealth != null && playerMana != null)
            {
                isAbsorbed = true;
                playerHealth.RestoreHealth(healthRestoreAmount);
                playerMana.RecoverMana(manaRestoreAmount);

                if (itemToAdd != null)
                {
                    Inventory.Instance.TryAddItem(itemToAdd, quantity);
                    Debug.Log($"{quantity}x {itemToAdd.Name} added to inventory.");
                }
                else
                {
                    Debug.LogWarning("Item to add is not assigned in lifepot!");
                }

                StartCoroutine(MoveToPlayerAndDeactivate(other.transform));
            }
        }
    }

    private IEnumerator MoveToPlayerAndDeactivate(Transform target)
    {
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(deactivateDelay);
        gameObject.SetActive(false); 
        BackgroundSoundManager.Instance.PlayCollectSFX();
    }
}
