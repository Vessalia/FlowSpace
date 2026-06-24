using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;

public class BeatManager : MonoSingleton<BeatManager>
{
	[StructLayout(LayoutKind.Sequential)]
	private class TimelineInfo
	{
		public float tempo = 0;
		public int currentBeat = 0;
		public FMOD.StringWrapper lastMarker = new();
	}

	private TimelineInfo timelineInfo = new TimelineInfo();
	private GCHandle timelineHandle;

	// Beats
	private EVENT_CALLBACK beatCallback = new EVENT_CALLBACK(BeatEventCallback);
	private Dictionary<Tuple<float, float>, Action> beatHandlers = new();

	// Markers
	private Dictionary<string, Action> markerHandlers = new();

	protected override void Awake()
	{
		timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
		TrackAudio(AudioManager.Instance.GetMusicInstance()); // This is terrible, but should work for now
	}

	void OnDestroy()
	{
		timelineHandle.Free();
	}

	public void TrackAudio(EventInstance audioInstance)
	{
		audioInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
		audioInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
	}

	public void Update()
	{
		foreach (var handlerPair in beatHandlers)
		{
			var handler = handlerPair.Value;

			handler.Invoke();
		}
	}

	public void RegisterBeatListener(Action listener, float beat, float offset = 0)
	{
		offset = Mathf.Repeat(offset, 1);
		var beatSig = new Tuple<float, float>(beat, offset);

		if (beatHandlers.ContainsKey(beatSig))
		{
			beatHandlers[beatSig] += listener;
		}
		else
		{
			beatHandlers.Add(beatSig, listener);
		}
	}

	public void DeregisterBeatListener(Action listener, float beat, float offset = 0)
	{
		offset = Mathf.Repeat(offset, 1);
		var beatSig = new Tuple<float, float>(beat, offset);

		if (beatHandlers.ContainsKey(beatSig))
		{
			beatHandlers[beatSig] -= listener;

			if (beatHandlers[beatSig] == null)
			{
				beatHandlers.Remove(beatSig);
			}
		}
	}

	public void RegisterMarkerListener(string marker, Action listener, bool autoDeregister = false)
	{
		if (autoDeregister)
		{
			listener += () => DeregisterMarkerListener(marker, listener);
		}

		if (markerHandlers.ContainsKey(marker))
		{
			markerHandlers[marker] += listener;
		}
		else
		{
			markerHandlers.Add(marker, listener);
		}
	}

	public void DeregisterMarkerListener(string marker, Action listener)
	{
		if (markerHandlers.ContainsKey(marker))
		{
			markerHandlers[marker] -= listener;

			if (markerHandlers[marker] == null)
			{
				markerHandlers.Remove(marker);
			}
		}

	}

	[AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	private static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
	{
		EventInstance instance = new(instancePtr);

		IntPtr timelineInfoPtr;
		FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

		if (result != FMOD.RESULT.OK)
		{
			Debug.LogError("Timeline Callback error: " + result);
		}
		else if (timelineInfoPtr != IntPtr.Zero)
		{
			GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
			TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

			switch (type)
			{
				case EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
				{
					var parameter = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
					timelineInfo.currentBeat = parameter.beat;
					timelineInfo.tempo = parameter.tempo;
					break;
				}
				case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
				{
					var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
					timelineInfo.lastMarker = parameter.name;
					break;
				}
			}
		}

		return FMOD.RESULT.OK;
	}
}
