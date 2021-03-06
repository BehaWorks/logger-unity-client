﻿using System.Collections.Generic;
using System.Linq;
using LoggerServer.Models;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MovementsSimulator : MonoBehaviourWithPrint
{
    public Canvas menuHolder;
    public TextMeshProUGUI messageHolder;
    public Button continueButton;

    public Material material;
    public float partMaxDurationInSeconds = 1f;
    public float gestureScale = 5f;
    public int countToGenerate = 25;

    private static readonly string[] _userNames =
    {
        "h_matusk",
        "h_andrej",
        "h_ada",
        "h_zuzka",
        "h_martin",
        "h_vilo",
        "h_matus",
        "h_lukas",
        "h_janka"
    };

    private int _currentUserIndex;

    private GameObject _holder;
    private TrailRenderer _trail;
    private List<MovementModel> _movements;
    private int _movementIndex;
    private float _partCurrentDurationInSeconds;

    private bool _finished = true;

    private readonly Dictionary<string, UserGestures> _userGestures = LoadUserGestures();

    private void Update()
    {
        var currentUserName = _userNames[_currentUserIndex];

        if (_finished && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Destroy(_holder);

            _movements = _userGestures[currentUserName].CurrentGesture;

            if (_movements == null)
            {
                return;
            }

            _holder = new GameObject("Simulations Bundle");
            _holder.transform.parent = transform;
            _partCurrentDurationInSeconds = partMaxDurationInSeconds;

            _trail = _holder.AddComponent<TrailRenderer>();
            _trail.material = material;

            _trail.widthMultiplier = 0.15f;
            _trail.time = 0.1f;

            _finished = false;
        }

        if (!_finished)
        {
            var from = _movements[_movementIndex];
            var fromPosition = new Vector3((float)from.X, (float)from.Y, (float)from.Z);

            var to = _movements[_movementIndex + 1];
            var toPosition = new Vector3((float)to.X, (float)to.Y, (float)to.Z);

            var currentPosition = _holder.transform.position = Vector3.Lerp(
                fromPosition,
                toPosition,
                _partCurrentDurationInSeconds / partMaxDurationInSeconds);

            _trail.startColor = ColorFromPosition(currentPosition);
            _trail.endColor = ColorFromPosition(currentPosition);

            if (_partCurrentDurationInSeconds >= partMaxDurationInSeconds)
            {
                var lineHolder = new GameObject($"Line from {from.X} | {from.Y} | {from.Z}");
                lineHolder.transform.parent = _holder.transform;

                var line = lineHolder.AddComponent<LineRenderer>();
                line.material = material;

                var positions = new[] { fromPosition, toPosition };
                line.SetPositions(positions);

                line.widthMultiplier = 0.1f;
                line.startColor = ColorFromPosition(fromPosition);
                line.endColor = ColorFromPosition(toPosition);

                _movementIndex++;
                _partCurrentDurationInSeconds = 0;
            }

            _partCurrentDurationInSeconds += Time.deltaTime;
        }

        if (_movementIndex == _movements.Count)
        {
            ShowMenu($"Welcome {currentUserName}.", true);
            _finished = true;
            _movementIndex = 0;
            _currentUserIndex = (_currentUserIndex + 1) % _userNames.Length;
        }
    }

    private static Dictionary<string, UserGestures> LoadUserGestures()
    {
        return _userNames
            .Select(name => new UserGestures(name))
            .ToDictionary(gestures => gestures.UserId, gestures => gestures);
    }

    private Color ColorFromPosition(Vector3 position)
    {
        return new Color(
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.x / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.y / gestureScale / 2 + 0.5f),
            Mathf.Lerp(-gestureScale / 2, gestureScale / 2, position.z / gestureScale / 2 + 0.5f));
    }

    private void ShowMenu(string message, bool success)
    {
        menuHolder.gameObject.SetActive(true);
        messageHolder.text = message;
        continueButton.gameObject.SetActive(success);
    }

    public void Repeat()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RegistrationScene()
    {
        SceneManager.LoadScene("Registration Keyboard");
    }

    public void Continue()
    {
        Print("Not implemented yet!"); // TODO
    }
}
