using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RichTap.Common
{
	[System.Serializable]
	public class HeMeta
	{
		public int Version;
		public string Created;
		public string Description;
	}

	[System.Serializable]
	public class HeHEAD
	{
		public HeMeta Metadata;
	}

	[System.Serializable]
	public class CurveItem
	{
		public int Time;
		public double Intensity;
		public int Frequency;
	}

	[System.Serializable]
	public class Parameters
	{
		public int Intensity;
		public int Frequency;
		public List<CurveItem> Curve;
	}

	[System.Serializable]
	public class Event
	{
		public int Duration;
		public Parameters Parameters;
		public string Type;
		public int RelativeTime;
		public int Index;
	}

	[System.Serializable]
	public class PatternItem
	{
		public Event Event;
	}

	[System.Serializable]
	public class HeFormat10
	{
		public HeMeta Metadata;
		public List<PatternItem> Pattern;
	}

	[System.Serializable]
	public class PatternListItem
	{
		public int AbsoluteTime;
		public List<PatternItem> Pattern;
	}

	[System.Serializable]
	public class HeFormat20
	{
		public HeMeta Metadata;
		public List<PatternListItem> PatternList;
	}

}
