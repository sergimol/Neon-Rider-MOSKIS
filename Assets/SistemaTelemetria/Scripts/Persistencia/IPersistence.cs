using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPersistence : MonoBehaviour
{
    public abstract void Send(TrackerEvent e);

    public abstract void Flush();
}