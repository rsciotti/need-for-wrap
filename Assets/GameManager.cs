using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private WrapController _wrapController;
        [SerializeField] private GameObject _aiCarControllerGameObj;

        private PlayerInputManager _playerInputManager;

        private List<PlayerInput> _playerList;
        private List<GameObject> _aiCarObjList;

        private void Awake()
        {
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
            _playerList.Add(playerInput);
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
                _wrapController.PopAtLocation(player.transform.position);
            }

            foreach (GameObject aiCar in _aiCarObjList) {
                _wrapController.PopAtLocation(aiCar.transform.position);
            }
        }
    }
}