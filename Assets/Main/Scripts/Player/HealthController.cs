using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class HealthController : MonoBehaviour
{
    public UnityEvent<int> healthChangeEvent;

    [SerializeField]
    private int health = 10;

    [SerializeField]
    private int maxHealth = 10;

    [SerializeField]
    private Vector2 healthBarOffset = new Vector2(-4f, 1f);

    [SerializeField]
    private GameObject healthBar;

    [SerializeField]
    private float healthBarShowTimeSeconds = 2f;

    private GameObject healthBarBackground;
    private GameObject healthBarForeground;

    private float maxHealthXScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (healthChangeEvent == null) {
            healthChangeEvent = new UnityEvent<int>();
        }

        GameObject healthBarCopy = Instantiate(healthBar, gameObject.transform.position, Quaternion.identity);
        healthBarCopy.transform.parent = gameObject.transform;
        healthBarCopy.transform.position += new Vector3(healthBarOffset.x, healthBarOffset.y, 0f);
        healthBarBackground = healthBarCopy.transform.GetChild(0).gameObject;
        healthBarForeground = healthBarCopy.transform.GetChild(1).gameObject;
        maxHealthXScale = healthBarBackground.transform.localScale.x;

        UpdateHealth(health);
    }

    public void Damage(int amount) {
        Debug.Log("Damage called!");
        UpdateHealth(health - amount);
    }

    public void Heal(int amount) {
        UpdateHealth(health + amount);
    }

    public int GetHealth() {
        return health;
    }

    private void UpdateHealth(int newHealth) {
        health = newHealth;
        if (health < 0) {
            health = 0;
        }
        float newXScale = ((float) newHealth / maxHealth) * healthBarBackground.transform.localScale.x;
        healthBarForeground.transform.localScale = new Vector3(newXScale, healthBarBackground.transform.localScale.y, 1f);
        healthChangeEvent.Invoke(health);
        StartCoroutine(ShowHealthBarTemporarily());
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    private IEnumerator ShowHealthBarTemporarily() {
        SetHealthBarVisibile(true);
        yield return new WaitForSeconds(healthBarShowTimeSeconds);
        SetHealthBarVisibile(false);
    }

    private void SetHealthBarVisibile(bool visible) {
        healthBarBackground.GetComponent<SpriteRenderer>().enabled = visible;
        healthBarForeground.GetComponent<SpriteRenderer>().enabled = visible;
    }
}
