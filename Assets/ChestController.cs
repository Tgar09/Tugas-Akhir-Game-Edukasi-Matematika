using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject objectToActivate;
    public Animator objectAnimator;
    public float moveDistance = 1f;
    public float moveSpeed = 2f;

    private bool chestOpened = false;

    private void Start()
    {
        if (objectToActivate != null && objectAnimator == null)
        {
            objectAnimator = objectToActivate.GetComponent<Animator>();
        }

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!chestOpened && other.CompareTag("Player")) // Menggunakan CompareTag untuk memeriksa tag
        {
            OpenChest();
            Debug.Log("Player masuk ke jangkauan peti! (2D)");
        }
    }

    private void OpenChest()
    {
        chestOpened = true;

        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            StartCoroutine(MoveObjectUp(objectToActivate, moveDistance, moveSpeed));
        }

        if (objectAnimator != null)
        {
            objectAnimator.SetBool("open", true);
            BackgroundSoundManager.Instance.PlayChestOpenSFX();
        }
        Debug.Log("Peti dibuka");
    }

    private System.Collections.IEnumerator MoveObjectUp(GameObject obj, float distance, float speed)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = startPos + new Vector3(0, distance, 0);
        
        while (Vector3.Distance(obj.transform.position, targetPos) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        obj.transform.position = targetPos; 
    }
}
