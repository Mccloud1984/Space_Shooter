using System;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
    public static class GameConfig
    {
        public const string AUDIO_MANAGER_NAME = "Audio_Manager";
        private static AudioClipsManager audioClips;
        public static AudioClipsManager GetAudioClipsManager()
        {
            audioClips = audioClips ?? GameObject.Find(AUDIO_MANAGER_NAME)?.GetComponent<AudioClipsManager>();
            if (audioClips == null)
                Debug.Log($"Failed to get AudioClips on {AUDIO_MANAGER_NAME}");
            return audioClips;
        }


        private static GameManager _gameManager;
        public static GameManager GetGameManager()
        {
            if(_gameManager == null)
                _gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
            if (_gameManager == null)
                Debug.LogError("Failed to find game manager.");
            return _gameManager;
        }

        private static UIManager _uiManager;
        public static UIManager GetUIManager()
        {
            if(_uiManager == null)
                _uiManager = GameObject.FindGameObjectWithTag("UIManager")?.GetComponent<UIManager>();
            if (_uiManager == null)
                Debug.LogError("Failed to find UI Manager.");
            return _uiManager;
        }

        private static SpawnManager _spawnManager;
        public static SpawnManager GetSpawnManager()
        {
            if(_spawnManager == null)
                _spawnManager =  GameObject.Find("Spawn_Manager")?.GetComponent<SpawnManager>();
            if (_spawnManager == null)
                Debug.LogError("Failed to find spawn manager.");
            return _spawnManager;
        }

        private static bool _coopModeEnabled = false;

        public static bool IsCoOpMode()
        {
            return _coopModeEnabled;
        }

        public static void SetCoOpMode(bool isCoOpMode)
        {
            _coopModeEnabled = isCoOpMode;
        }

        public static void ResetGame()
        {
            audioClips = null;
            _uiManager = null;
            _spawnManager = null;
        }
    }

}