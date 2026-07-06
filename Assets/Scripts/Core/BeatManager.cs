using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct BeatSignature
{
	public float Period {  get; private set; }
	public float Offset { get; private set; }

	public BeatSignature(float period, float offset) : this()
	{
		this.Period = period;
		this.Offset = offset;
	}
};

public class BeatManager : Singleton<BeatManager>
{
	[StructLayout(LayoutKind.Sequential)]
	private class TimelineInfo
	{
		public float tempo = 0;
		public int currentBeat = 0;
		public FMOD.StringWrapper lastMarker = new();

		public bool beatDirty = false;
		public bool markerDirty = false;
	}

	private TimelineInfo timelineInfo = null;
	private GCHandle timelineHandle;

	ChannelGroup trackedGroup;

	ulong lastClock = 0;
	ulong clock = 0;

	float bpm = 0;
	int sampleRate = 0;

	bool trackingAudio = false;

	// Beats
	private EVENT_CALLBACK beatCallback;
	private Dictionary<BeatSignature, BeatEvent> beatHandlers = new();
	private Dictionary<BeatSignature, bool> primed = new();

	// Markers
	private Dictionary<string, Action> markerHandlers = new();

	BeatManager()
	{
		beatCallback = new EVENT_CALLBACK(BeatEventCallback);
	}

	~BeatManager()
	{
		timelineHandle.Free();
	}

	public void TrackAudio(EventInstance audioInstance)
	{
		timelineInfo = new TimelineInfo();
		timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

		audioInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
		audioInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
		audioInstance.getChannelGroup(out trackedGroup);

		// get the sample rate
		RuntimeManager.CoreSystem.getSoftwareFormat(out sampleRate, out var speakerMode, out var numRawSpeakers);
		UpdateClock();
		lastClock = clock; // only track events from the current position of the song

		trackingAudio = true;
	}

	private float GetSecondsPerBeat(float bpm)
	{
		return bpm == 0 ? -1 : 60 / bpm;
	}

	public void Update()
	{
		if (!trackingAudio) return;

		UpdateClock();
		FireHandlers();
		ClearFlags();
	}

	private void UpdateClock()
	{
		lastClock = clock;
		trackedGroup.getDSPClock(out clock, out var parentClock);
	}

	private void FireHandlers()
	{
		float secondsPerBeat = GetSecondsPerBeat(bpm);
		if (secondsPerBeat < 0) return; // tempo not set yet

		float samplesPerBeat = secondsPerBeat * sampleRate;
		float prevBeatPos = lastClock / samplesPerBeat;
		float currBeatPos = clock / samplesPerBeat;

		foreach (var handlerPair in beatHandlers)
		{
			var signature = handlerPair.Key;
			bool inWindow = IsActive(signature, prevBeatPos, currBeatPos, out float beatDelay);

			if (inWindow && primed[signature])
			{
				handlerPair.Value.Invoke(beatDelay * secondsPerBeat);
				primed[signature] = false;
			}
			else if (!inWindow && !primed[signature])
			{
				primed[signature] = true;
			}
		}
	}

	private void ClearFlags()
	{
		if (timelineInfo.beatDirty)
		{
			bpm = timelineInfo.tempo;
		}

		if (timelineInfo.markerDirty && markerHandlers.ContainsKey(timelineInfo.lastMarker))
		{
			markerHandlers[timelineInfo.lastMarker].Invoke();
		}
	}

	private bool IsActive(BeatSignature signature, float prevBeatPos, float currBeatPos, out float beatDelay)
	{
		// get smallest n s.t. offset + n * period > prev → n > (prev - offset) / period
		float n = Mathf.Floor((prevBeatPos - signature.Offset) / signature.Period) + 1;
		beatDelay = currBeatPos - signature.Offset + n * signature.Period;
		return beatDelay >= 0;
	}

	public void RegisterBeatListener(Action<float> listener, float period, float offset = 0)
	{
		offset = Mathf.Repeat(offset, 1);
		var beatSig = new BeatSignature(period, offset);

		if (beatHandlers.ContainsKey(beatSig))
		{
			beatHandlers[beatSig].RegisterBeatListener(listener);
		}
		else
		{
			BeatEvent handler = new(period, offset);
			handler.RegisterBeatListener(listener);
			beatHandlers.Add(beatSig, handler);
			primed.Add(beatSig, true);
		}
	}

	public void DeregisterBeatListener(Action<float> listener, float period, float offset = 0)
	{
		offset = Mathf.Repeat(offset, 1);
		var beatSig = new BeatSignature(period, offset);

		if (beatHandlers.ContainsKey(beatSig))
		{
			beatHandlers[beatSig].DeregisterBeatListener(listener);

			if (!beatHandlers[beatSig].HasAction)
			{
				beatHandlers.Remove(beatSig);
				primed.Remove(beatSig);
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
			UnityEngine.Debug.LogError("Timeline Callback error: " + result);
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
					timelineInfo.beatDirty = true;
					break;
				}
				case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
				{
					var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
					timelineInfo.lastMarker = parameter.name;
					timelineInfo.markerDirty = true;
					break;
				}
			}
		}

		return FMOD.RESULT.OK;
	}
}
