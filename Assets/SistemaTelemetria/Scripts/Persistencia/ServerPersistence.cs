using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using System.IO;
using System.Net.Security;
using System.Runtime.ConstrainedExecution;
using System;

public class ServerPersistence : IPersistence
{

    List<TrackerEvent> eventsBuff;

    public string url = "https://neon-rider-ae6c3-default-rtdb.europe-west1.firebasedatabase.app/";

    JSONSerializer serializerJSON = null;   //JSON
    DatabaseReference reference;

    private int counterEvents = 0;

    string idOrdenador = "NoId";

    private void Awake()
    {
        idOrdenador = SystemInfo.deviceUniqueIdentifier;

        AppOptions options = new AppOptions
        {
            ApiKey = "AIzaSyDxEcT2zNZyKJ6_PSwVgTHh14IQdgf7EFM",
            DatabaseUrl = new System.Uri(url),
            AppId = "neon-rider-ae6c3",
            StorageBucket = "neon-rider-ae6c3.appspot.com",
            MessageSenderId = "989654265255",
            ProjectId ="1:989654265255:web:d43dfdace0abe66d309a82"
        };
        FirebaseApp app = FirebaseApp.Create(options);
        
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        eventsBuff = new();

        serializerJSON = GetComponent<JSONSerializer>();
    }

    private void Start()
    {
        
    }
    public override void Send(TrackerEvent e) {
        eventsBuff.Add(e);
    }

    public override void Flush() {
        List<TrackerEvent> events = new List<TrackerEvent>(eventsBuff);
        eventsBuff.Clear();
        WriteAsync(events);
        reference.Push();
    }

    private void OnDestroy()
    {
    }
    private async void WriteAsync(List<TrackerEvent> events)
    {
        foreach (TrackerEvent e in events)
        {
            string ser = serializerJSON.Serialize(e);
            await reference.Child("PcId: " + idOrdenador).Child("SessionId: " + Tracker.instance.getSessionId().ToString()).Child(counterEvents.ToString()).SetRawJsonValueAsync(ser.TrimStart('['));
            counterEvents++;
        }
    }
}