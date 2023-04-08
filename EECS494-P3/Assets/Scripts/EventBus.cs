/* EventBus.cs
 * 
 * This script implements an "Event Bus" -- a critical part of the Pub/Sub design pattern.
 * Developers should make heavy use of the Subscribe() and Publish() methods below to receive and send
 * instances of your own, custom "event" classes between systems. This "loosely couples" the systems, preventing spaghetti.
 * 
 * Please find an example usage of Publish() in ScorePointOnTouch.cs
 * Please find an example, custom Event class in ScorePointOnTouch.cs
 * Please find an example usage of Subscribe() in ScoreUI.cs
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class EventBus {
    /* DEVELOPER : Change this to "true" and all events will be logged to console automatically */
    public const bool DEBUG_MODE = false;

    private static readonly Dictionary<Type, IList> _topics = new();

    public static void Publish<T>(T published_event) {
        /* Use type T to identify correct subscriber list (correct "topic") */
        var t = typeof(T);

        switch (DEBUG_MODE) {
            case true:
                Debug.Log("[Publish] event of type " + t + " with contents (" + published_event.ToString() + ")");
                break;
        }

        if (_topics.ContainsKey(t)) {
            IList subscriber_list = new List<Subscription<T>>(_topics[t].Cast<Subscription<T>>());

            switch (DEBUG_MODE) {
                /* iterate through the subscribers and pass along the event T */
                case true:
                    Debug.Log("..." + subscriber_list.Count + " subscriptions being executed for this event.");
                    break;
            }

            /* This is a collection of subscriptions that have lost their target object. */
            var orphaned_subscriptions = new List<Subscription<T>>();

            foreach (Subscription<T> s in subscriber_list)
                if (s.callback.Target == null || s.callback.Target.Equals(null))
                    /* This callback is hanging, as its target object was destroyed */
                    /* Collect this callback and remove it later */
                    orphaned_subscriptions.Add(s);
                else
                    s.callback(published_event);

            /* Unsubcribe orphaned subs that have had their target objects destroyed */
            foreach (var orphan_subscription in orphaned_subscriptions) Unsubscribe(orphan_subscription);
        }
        else {
            switch (DEBUG_MODE) {
                case true:
                    Debug.Log("...but no one is subscribed to this event right now.");
                    break;
            }
        }
    }

    public static Subscription<T> Subscribe<T>(Action<T> callback) {
        /* Determine event type so we can find the correct subscriber list */
        var t = typeof(T);
        var new_subscription = new Subscription<T>(callback);

        _topics[t] = _topics.ContainsKey(t) switch {
            /* If a subscriber list doesn't exist for this event type, create one */
            false => new List<Subscription<T>>(),
            _ => _topics[t]
        };

        _topics[t].Add(new_subscription);

        switch (DEBUG_MODE) {
            case true:
                Debug.Log("[Subscribe] subscription of function (" + callback.Target.ToString() + "." +
                          callback.Method.Name + ") to type " + t + ". There are now " + _topics[t].Count +
                          " subscriptions to this type.");
                break;
        }

        return new_subscription;
    }

    public static void Unsubscribe<T>(Subscription<T> subscription) {
        var t = typeof(T);

        switch (DEBUG_MODE) {
            case true:
                Debug.Log("[Unsubscribe] attempting to remove subscription to type " + t);
                break;
        }

        if (_topics.ContainsKey(t) && _topics[t].Count > 0) {
            _topics[t].Remove(subscription);

            switch (DEBUG_MODE) {
                case true:
                    Debug.Log("...there are now " + _topics[t].Count + " subscriptions to this type.");
                    break;
            }
        }
        else {
            switch (DEBUG_MODE) {
                case true:
                    Debug.Log("...but this subscription is not currently valid (perhaps you already unsubscribed?)");
                    break;
            }
        }
    }
}

/* A "handle" type that is returned when the EventBus.Subscribe() function is used.
 * Use this handle to unsubscribe if you wish via EventBus.Unsubscribe */
public class Subscription<T> {
    public Action<T> callback { get; private set; }

    public Subscription(Action<T> _callback) {
        callback = _callback;
    }

    ~Subscription() {
        EventBus.Unsubscribe(this);
    }
}