using UnityEngine;
using UnityEngine.UI;

public class BubblesPoppedCounterScript : MonoBehaviour
{
    public int[] bubblesPopped;
    public Text[] BubblesPoppedText;

    [ContextMenu("incrementPopped")]
    public void incrementPopped(int playerNum)
    {
        if (playerNum < 0 || playerNum >= bubblesPopped.Length) {
            return;
        }

        bubblesPopped[playerNum]++;
        BubblesPoppedText[playerNum].text = bubblesPopped[playerNum].ToString();
    }

    public void decrementPopped(int playerNum, int amount) {
        if (playerNum < 0 || playerNum >= bubblesPopped.Length) {
            return;
        }

        bubblesPopped[playerNum] -= amount;
        BubblesPoppedText[playerNum].text = bubblesPopped[playerNum].ToString();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        for (int i = 0; i < 6; i++)
        {
            BubblesPoppedText[i].text = bubblesPopped[i].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
