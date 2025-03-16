using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Reflection;
using Unity.VisualScripting;


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
        private List<InputDevice> _playerDevices;
        private List<int> _playerDeviceIds;
        private List<InputDevice> _disconnectedDevices;
        private List<int> _disconnectedDeviceIds;

        private float timer = 6f;
        private float interval = 8f; // Time interval in seconds

        [SerializeField] private AudioClip[] music;

        //music assigned to Assets/Main/Prefabs/Globals/GameManager.prefab
        private enum MusicIndex
        {
            GameTrack120, //Assets/Main/Audio/Music/FunInAWarehouse120.mp3
            GameTrack130, //Assets/Main/Audio/Music/FunInAWarehouse130.mp3
            GameTrack140, //Assets/Main/Audio/Music/FunInAWarehouse140.mp3
            GameTrack150 //Assets/Main/Audio/Music/FunInAWarehouse.mp3
        }

        private bool firstUpdate = true;
        private int highScore = 0;
        private int musicSpeedThreshold;

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
                InputSystem.onDeviceChange += OnDeviceChange;
                _playerList = new();
                _aiCarObjList = new();
                _playerDevices = new();
                _playerDeviceIds = new();
                _disconnectedDevices = new();
                _disconnectedDeviceIds = new();

                Instance = this;
            }
        }

        private void OnDeviceChange(InputDevice dev, InputDeviceChange change)
        {
            //unplugging and replugging fire multiple events
            //use removed and reconnected for matching with players
            switch (change)
            {
                case InputDeviceChange.Removed:
                    Debug.Log("Device removed!");
                    _disconnectedDevices.Add(dev);
                    _disconnectedDeviceIds.Add(dev.deviceId);
                    break;
                case InputDeviceChange.Reconnected:
                    Debug.Log("Device reconnected!");
                    if (_disconnectedDevices.Contains(dev))
                    {
                        //device object stored in _disconnectedDevices will have the new deviceID
                        Debug.Log("Device match! " + _disconnectedDevices[_disconnectedDevices.IndexOf(dev)] +
                                  _disconnectedDevices[_disconnectedDevices.IndexOf(dev)].displayName +
                                  _disconnectedDevices[_disconnectedDevices.IndexOf(dev)].deviceId);
                        if (_playerDevices.Contains(dev)) _playerDeviceIds[_playerDevices.IndexOf(dev)] = dev.deviceId;
                        _disconnectedDeviceIds.RemoveAt(_disconnectedDevices.IndexOf(dev));
                        _disconnectedDevices.Remove(dev);
                    }
                    break;
                default:
                    break;
            }

            Debug.Log("Changed: " + dev + " Name " + dev.displayName + " DevId " + dev.deviceId);

            /*
            int i = -1;
            foreach (InputDevice id in InputSystem.devices)//playerInput.devices)
            {
                i++;
                Debug.Log("Dev " + i + ": " + id);
                Debug.Log("Name: " + id.displayName +
                " DevId: " + id.deviceId +
                " Manuf: " + id.description.manufacturer +
                " Prod: " + id.description.product +
                " Vers: " + id.description.version +
                " Serial: " + id.description.serial);
            }
            foreach (PlayerInput pi in _playerList)
            {
                if (pi.devices[0].deviceId == dev.deviceId)
                    Debug.Log("Player match! " + _playerList.IndexOf(playerInput));
            }
            */
        }

        // Called when a player joins
        private void OnPlayerJoined(PlayerInput playerInput)
        {
            //player already using controller = respawning after death
            if (_playerDevices.Contains(playerInput.devices[0]))
            {
                _playerList[_playerDevices.IndexOf(playerInput.devices[0])] = playerInput;
                _playerDeviceIds[_playerDevices.IndexOf(playerInput.devices[0])] = playerInput.devices[0].deviceId;
            }
            //if all colors were assigned and any players have disconnected controllers
            else if (_playerList.Count == _playerInputManager.maxPlayerCount &&
                     _playerDevices.Intersect(_disconnectedDevices).ToList().Any())
            {
                //first dead player with disconnection gets the new controller
                for (int i = 0; i < _playerList.Count; i++)
                {
                    if (!_playerList[i].isActiveAndEnabled && _disconnectedDeviceIds.Contains(_playerDeviceIds[i]))
                    {
                        _playerList[i] = playerInput;
                        _playerDevices[i] = playerInput.devices[0];
                        _playerDeviceIds[i] = playerInput.devices[0].deviceId;
                        i = _playerList.Count;
                    }
                }
            }
            //new player color
            else
            {
                _playerList.Add(playerInput);
                _playerDevices.Add(playerInput.devices[0]);
                _playerDeviceIds.Add(playerInput.devices[0].deviceId);
            }

            SpriteRenderer spriteRenderer = playerInput.gameObject.GetComponent<SpriteRenderer>();
            if (_playerList.Count <= _playerInputManager.maxPlayerCount)
                spriteRenderer.sprite = _availableCarSprites[_playerList.IndexOf(playerInput)];

            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;

            ListenToPlayerHealthChange(playerInput);
        }
        
        private void OnPlayerLeft(PlayerInput playerInput)
        {
            Debug.Log($"Player {_playerList.IndexOf(playerInput)} has left the game.");
        }

        private void ListenToPlayerHealthChange(PlayerInput playerInput) {
            if (_playerList.Count > 0) {
               HealthController healthController = _playerList[_playerList.IndexOf(playerInput)].GetComponent<HealthController>();
               if (healthController != null) {
                    healthController.healthChangeEvent.AddListener((health) => { OnPlayerHealthChange(health, _playerList.IndexOf(playerInput)); });
               }
            }
        }

        private void OnPlayerHealthChange(int health, int index) {
            if (health == 0) {
                _popCounter.decrementPopped(index, _deathPenalty);
            }
        }

        public void boom(Vector3 pos, float radius, bool inverse)
        {
            if (inverse)
                _wrapController.SetWithinRadius(pos, radius * 2f, false);
            else
                _wrapController.SetWithinRadius(pos, radius, true);
        }

        private void OnDestroy()
        {
            if (_playerInputManager != null)
            {
                _playerInputManager.onPlayerJoined -= OnPlayerJoined;
                _playerInputManager.onPlayerLeft -= OnPlayerLeft;
                InputSystem.onDeviceChange -= OnDeviceChange;
            }
        }

        private void Update()
        {

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (firstUpdate)
            {
                SoundManager.Instance.PlayMusic(music[(int)MusicIndex.GameTrack120]);
                musicSpeedThreshold = winningScore / 4;
                firstUpdate = false;
            }

            timer += Time.deltaTime;
            
            if (timer >= interval)
            {
                _aiCarObjList.Add(Instantiate(_aiCarControllerGameObj,
                                  new Vector3(UnityEngine.Random.Range(-10f, 10f),
                                              UnityEngine.Random.Range(-10f, 10f), 0), Quaternion.identity));
                
                // Reset the timer
                timer = 0f;
            }
            
            foreach (PlayerInput player in _playerList)
            {
                if (player.GetComponent<BaseVehicle>().gameObject.activeSelf)
                {
                    if (_wrapController.SetAtLocation(player.transform.position,
                                                      !player.GetComponent<BaseVehicle>().GetInverseState()))
                    {
                        _popCounter.incrementPopped(_playerList.IndexOf(player));

                        if (_playerList.IndexOf(player) >= 0 && _playerList.IndexOf(player) < _popCounter.bubblesPopped.Length)
                        {
                            if (_popCounter.bubblesPopped[_playerList.IndexOf(player)] >= winningScore)
                            {
                                winPanel.SetActive(true);
                                color = playerColors[_playerList.IndexOf(player)];
                                string text = "Player " + color + " wins!\n Popped " + winningScore + " bubbles!";
                                winText.text = text;
                                Time.timeScale = 0f; // Pause the game
                            }
                            else if (_popCounter.bubblesPopped[_playerList.IndexOf(player)] > highScore)
                                highScore = _popCounter.bubblesPopped[_playerList.IndexOf(player)];
                        }
                    }
                }
            }

            if (highScore >= musicSpeedThreshold)
            {
                switch (musicSpeedThreshold)
                {
                    case int s when s == winningScore / 4:
                        SoundManager.Instance.NewMusicSpeed(music[(int)MusicIndex.GameTrack130]);
                        break;
                    case int s when s == winningScore / 4 * 2:
                        SoundManager.Instance.NewMusicSpeed(music[(int)MusicIndex.GameTrack140]);
                        break;
                    case int s when s == winningScore / 4 * 3:
                        SoundManager.Instance.NewMusicSpeed(music[(int)MusicIndex.GameTrack150]);
                        break;
                    default:
                        break;
                }
                musicSpeedThreshold += Math.Max(winningScore / 4, 1);
            }

            foreach (GameObject aiCar in _aiCarObjList) {
                _wrapController.SetAtLocation(aiCar.transform.position, !aiCar.GetComponent<BaseVehicle>().GetInverseState());
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