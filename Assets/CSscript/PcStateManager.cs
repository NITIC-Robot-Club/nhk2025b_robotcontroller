using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PcStateManager : MonoBehaviour
{
    [SerializeField] private CircleGraphManager cpu;
    [SerializeField] private CircleGraphManager ram;
    [SerializeField] private CircleGraphManager temp;
    [SerializeField] private CircleGraphManager net_up;
    [SerializeField] private CircleGraphManager net_down;
    [SerializeField] private TMP_Text tmp;
    private string text;
    void Update()
    {
        tmp.SetText("hostname : " + text);
    }
    public void UpdatePcState(nhk2025b_msgs.msg.PcState msg)
    {
        text = msg.Hostname;
        cpu.UpdateCircleGraph(msg.Cpu_percent, msg.Cpu_percent / 100);
        ram.UpdateCircleGraph(msg.Ram_percent, msg.Ram_percent / 100);
        temp.UpdateCircleGraph(msg.Temperature, msg.Temperature / 100);
        net_up.UpdateCircleGraph(msg.Net_up_mbps, msg.Net_up_mbps / 10);
        net_down.UpdateCircleGraph(msg.Net_down_mbps, msg.Net_down_mbps / 10);
    }
}