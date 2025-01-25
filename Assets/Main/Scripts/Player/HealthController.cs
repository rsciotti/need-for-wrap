using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    public UnityEvent<int> healthChangeEvent;

    [SerializeField]
    private int health = 5;

    [SerializeField]
    private int maxHealth = 10;

    private GameObject[] healthBubbleObjs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (healthChangeEvent == null) {
            healthChangeEvent = new UnityEvent<int>();
        }

        healthBubbleObjs = new GameObject[maxHealth];
        CreateHealthBar();
    }

    private void CreateHealthBar() {
        Vector2 position = gameObject.transform.position;
        foreach (GameObject healthBubble in healthBubbleObjs) {
            
        }
    }

    public void Damage(int amount) {
        SetHealth(health - amount);
    }

    public void Heal(int amount) {
        SetHealth(health + amount);
    }

    public int GetHealth() {
        return health;
    }

    private void SetHealth(int newHealth) {
        health = newHealth;
        healthChangeEvent.Invoke(health);
    }

    public int GetMaxHealth() {
        return maxHealth;
    }
}
