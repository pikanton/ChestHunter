using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public Sprite closeChest;
    public Sprite openChest;
    public float ChestOpenDelay = 1.0F;
    public PauseMenu pauseMenu;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(OpenChestWithDelay());
        }
    }
    private IEnumerator OpenChestWithDelay()
    {
        yield return new WaitForSeconds(ChestOpenDelay / 2);

        gameObject.GetComponent<SpriteRenderer>().sprite = openChest;

        yield return new WaitForSeconds(ChestOpenDelay);

        pauseMenu.TogglePauseMenu();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
