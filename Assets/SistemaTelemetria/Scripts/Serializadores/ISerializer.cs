using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ISerializer : MonoBehaviour
{
    public abstract string Serialize(TrackerEvent e);
}

