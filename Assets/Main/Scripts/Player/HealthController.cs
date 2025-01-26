using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    public UnityEvent<int> healthChangeEvent;

    [SerializeField]
    private int health = 5;

    [SerializeField]
    private int maxHealth = 10;

    [SerializeField]
    private Vector2 healthBarOffset;

    [SerializeField]
    private float healthBarHorizontalSpacing;

    [SerializeField]
    private Sprite healthBarSprite;

    [SerializeField]
    private Sprite missingHealthSprite;

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
        for (int i = 0; i < maxHealth; i++) {
            healthBubbleObjs[i] = new GameObject();
            healthBubbleObjs[i].name = "HealthGameObject(" + i + ")";
            healthBubbleObjs[i].transform.parent = gameObject.transform;
            healthBubbleObjs[i].transform.position = new Vector2(healthBarOffset.x + position.x + i * healthBarHorizontalSpacing, healthBarOffset.y + position.y);
            
            SpriteRenderer renderer = healthBubbleObjs[i].AddComponent<SpriteRenderer>();
            renderer.sprite = healthBarSprite;
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
