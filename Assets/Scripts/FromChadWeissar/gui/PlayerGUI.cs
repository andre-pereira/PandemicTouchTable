using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using DG.Tweening;
using System;

public class PlayerGUI : MonoBehaviour
{

    Player _player = null;
    public Player Player
    {
        get { if (_player == null) _player = PlayerList.playerAtPosition(Position); return _player; }
        set { _player = value; }
    }

    public int Position;

    void OnEnable()
    {
    }

    public void init()
    {
        _player = null;
    }

    bool _isAnimating = false;
    List<int> _drawnCards = new List<int>();

    public void draw()
    {
        if (_isAnimating) return;
        if (Player == null) return;
    }
    public void drawLater(float time)
    {
        _isAnimating = true;
        this.ExecuteLater(time, doneAnimating);
    }
    void doneAnimating()
    {
        _isAnimating = false;
        draw();
    }
}