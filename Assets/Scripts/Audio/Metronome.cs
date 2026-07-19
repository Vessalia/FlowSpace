using FMOD;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Metronome
{
	[StructLayout(LayoutKind.Sequential)]
	public class TimelineInfo
	{
		public float tempo = 0;
		public int beat = 0;
		public int position = 0;
		public int bar = 0;
		public int length = 0;
		public FMOD.StringWrapper lastMarker = new();

		public bool beatDirty = false;
		public bool markerDirty = false;
	}

	public TimelineInfo timelineInfo { get; private set; }
	private GCHandle timelineHandle;

	private EVENT_CALLBACK eventCallback;

	public event Action<int, float> OnBeat;
	public event Action<string> OnMarker;

	public Metronome(EventInstance audioInstance, int audioLength)
	{
		eventCallback = new EVENT_CALLBACK(EventCallback);

		timelineInfo = new TimelineInfo();
		timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);

		timelineInfo.length = audioLength;

		audioInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
		audioInstance.setCallback(eventCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT | EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
	}

	~Metronome()
	{
		timelineHandle.Free();
	}

	// if this is where we want to handle this, we shouldn't do it repeatedly, and just store the actions
	public void HandleFlags()
	{
		if (timelineInfo.beatDirty)
		{
			OnBeat?.Invoke(timelineInfo.beat, timelineInfo.tempo);
		}
		if (timelineInfo.markerDirty)
		{
			OnMarker?.Invoke(timelineInfo.lastMarker);
		}

		timelineInfo.beatDirty = false;
		timelineInfo.markerDirty = false;
	}

	[AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
	private static FMOD.RESULT EventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
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
						timelineInfo.beat = parameter.beat;
						timelineInfo.tempo = parameter.tempo;
						timelineInfo.position = parameter.position;
						timelineInfo.bar = parameter.bar;
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

#if UNITY_EDITOR
	void OnGUI()
	{
		var content = "";
		content += $"**Music Manager Debug**\n\n";
		content += $"Length (ms): {timelineInfo.length}, Tempo: {timelineInfo.tempo}\n\n";
		content += $"Playback Position (ms): {timelineInfo.position}\n";
		content += $"Current Bar: {timelineInfo.bar}, Current Beat: {timelineInfo.beat}\n\n";
		content += $"Last Marker: {timelineInfo.lastMarker}";

		GUI.Label(new Rect(10, 10, 300, 150), content, GUI.skin.textArea);
	}
#endif
}
