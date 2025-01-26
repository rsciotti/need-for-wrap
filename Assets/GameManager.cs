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

        private PlayerInputManager _playerInputManager;

        public List<Sprite> _availableCarSprites;
        private List<PlayerInput> _playerList;
        private List<GameObject> _aiCarObjList;

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
            _aiCarObjList.Add(Instantiate(_aiCarControllerGameObj, new Vector3(0, 0, 0), Quaternion.identity));
        }
        
        private void OnPlayerLeft(PlayerInput playerInput)
        {
            Debug.Log($"Player {playerInput.playerIndex} has left the game.");
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
            foreach (PlayerInput player in _playerList)
            {
                if (_wrapController.PopAtLocation(player.transform.position)) {
                    _popCounter.incrementPopped(player.playerIndex);

                    if(_popCounter.bubblesPopped[player.playerIndex] >= winningScore)
                    {
                        winPanel.SetActive(true);
                        string text = "Player " + color + " wins!\n Popped " + winningScore + " bubbles!";
                        winText.text = text;
                    }
                }
            }

            foreach (GameObject aiCar in _aiCarObjList) {
                _wrapController.PopAtLocation(aiCar.transform.position);
            }
        }
    }
}