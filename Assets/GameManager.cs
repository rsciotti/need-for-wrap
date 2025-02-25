using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


namespace Main.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public int winningScore = 600;
        public GameObject winPanel;
        public TextMeshProUGUI winText;

        private string[] playerColors = {"Green", "Orange", "Pink", "Purple", "Red", "Yellow"};
        private string color = "";

        [SerializeField] private WrapController _wrapController;
        [SerializeField] private GameObject _aiCarControllerGameObj;
        [SerializeField] private BubblesPoppedCounterScript _popCounter;
        [SerializeField] private int _deathPenalty = 25;

        private PlayerInputManager _playerInputManager;

        public List<Sprite> _availableCarSprites;
        private List<PlayerInput> _playerList;
        private List<GameObject> _aiCarObjList;
        
        private float timer = 6f;
        private float interval = 8f; // Time interval in seconds
        
        public WrapController GetWrapController() {
            return _wrapController;
        }

        private void Awake()
        {
            winPanel.SetActive(false);
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else
            {
                _playerInputManager = GetComponent <PlayerInputManager>();
                _playerInputManager.onPlayerJoined += OnPlayerJoined;
                _playerInputManager.onPlayerLeft += OnPlayerLeft;
                _playerList = new();
                _aiCarObjList = new();
                Instance = this;
            }
        }

        // Called when a player joins
        private void OnPlayerJoined(PlayerInput playerInput)
        {
            SpriteRenderer spriteRenderer = playerInput.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _availableCarSprites[playerInput.playerIndex];
            color = playerColors[playerInput.playerIndex];
            _playerList.Add(playerInput);
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;

            ListenToPlayerHealthChange(playerInput);
        }
        
        private void OnPlayerLeft(PlayerInput playerInput)
        {
            Debug.Log($"Player {playerInput.playerIndex} has left the game.");
        }

        private void ListenToPlayerHealthChange(PlayerInput playerInput) {
            if (_playerList.Count > 0) {
               HealthController healthController = _playerList[^1].GetComponent<HealthController>();
               if (healthController != null) {
                    healthController.healthChangeEvent.AddListener((health) => { OnPlayerHealthChange(health, _playerList.Count - 1); });
               }
            }
        }

        private void OnPlayerHealthChange(int health, int index) {
            if (health == 0) {
                _wrapController.PopWithinRadius(_playerList[index].transform.position, 3f);
                _popCounter.decrementPopped(index, _deathPenalty);
            }
        }

        private void OnDestroy()
        {
            if (_playerInputManager != null)
            {
                _playerInputManager.onPlayerJoined -= OnPlayerJoined;
                _playerInputManager.onPlayerLeft -= OnPlayerLeft;
            }
        }

        private void Update()
        {

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            
            timer += Time.deltaTime;
            
            if (timer >= interval)
            {
                _aiCarObjList.Add(Instantiate(_aiCarControllerGameObj, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0), Quaternion.identity));
                
                // Reset the timer
                timer = 0f;
            }
            
            foreach (PlayerInput player in _playerList)
            {
                if (_wrapController.PopAtLocation(player.transform.position)) {
                    _popCounter.incrementPopped(player.playerIndex);


                    if(player.playerIndex >= 0 && player.playerIndex < _popCounter.bubblesPopped.Length && _popCounter.bubblesPopped[player.playerIndex] >= winningScore)
                    {
                        winPanel.SetActive(true);
                        string text = "Player " + color + " wins!\n Popped " + winningScore + " bubbles!";
                        winText.text = text;
                        Time.timeScale = 0f; // Pause the game
                    }
                }
            }

            foreach (GameObject aiCar in _aiCarObjList) {
                _wrapController.PopAtLocation(aiCar.transform.position);
            }

            /*DEBUGSTART
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Screen clicked at: " + Input.mousePosition);
                //Debug.Log("ScreenToWorld: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Debug.Log("ScreenToView:" + Camera.main.ScreenToViewportPoint(Input.mousePosition));
                foreach (GameObject aiCar in _aiCarObjList)
                {
                    //Debug.Log("AI at: " + aiCar.transform.position);
                    if (Vector2.Distance((Vector2)aiCar.transform.position,
                                         (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)) <= 2.5f)
                    {
                        //Debug.Log("Clicked on AI at: " + aiCar.transform.position);
                        aiCar.GetComponent<AICarController>().selectOn();
                    }
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                //Debug.Log("Screen right-clicked at: " + Input.mousePosition);
                //Debug.Log("ScreenToWorld: " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //Debug.Log("ScreenToView:" + Camera.main.ScreenToViewportPoint(Input.mousePosition));
                foreach (GameObject aiCar in _aiCarObjList)
                {
                    //Debug.Log("AI at: " + aiCar.transform.position);
                    if (Vector2.Distance((Vector2)aiCar.transform.position,
                                         (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition)) <= 2.5f)
                    {
                        //Debug.Log("Right-clicked on AI at: " + aiCar.transform.position);
                        aiCar.GetComponent<AICarController>().selectOff();
                    }
                }
            }
            DEBUGEND*/
        }
    }
}