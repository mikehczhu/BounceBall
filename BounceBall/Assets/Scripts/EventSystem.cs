using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameEvent
{
    kill_a_master,
    update_master_cnt,
    update_bullet_cnt,
    show_no_bullet,
    show_win,
    use_a_bullet,
    player_die,
    game_over
}


public class EventSystem : MonoBehaviour {

    private static EventSystem m_instance;
    public static EventSystem Instance { get { return m_instance; } }

    private Dictionary<int, object> m_ListenerDict;

    private void Awake()
    {
        m_instance = this;
        m_ListenerDict = new Dictionary<int, object>();
    }


    public void Subscribe(int event_name, Action action)
    {
        m_ListenerDict.Add(event_name, action);
    }
    public void Publish(int event_name)
    {
        ((Action)m_ListenerDict[event_name])();
    }
}
