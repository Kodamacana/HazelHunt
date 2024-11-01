using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RichTap.Common
{
    public static class RichtapUtility
    {
        private const int CLIP_TRANSIENT_DURATION = 55;

        private static readonly int[] DURATIONS =
        {
            48, 149, 48, 48, 48, 230, 154, 233, 104, 48,
            48, 48, 48, 48, 48, 48, 48, 48, 93, 48, 98,
            108, 188, 224, 538, 27, 1650, 223, 188, 398,
            3729, 310, 1073, 305, 835, 145, 178, 158, 224, 142,
            178, 2887, 316, 898, 600, 258, 3600, 2202, 1203, 800
        };

        public static int GetPresetDuration(RichtapPreset preset)
        {
            return DURATIONS[(int)preset - 1];
        }

        public static int GetClipDuration(RichtapClip clip)
        {
            int duration = 0;
            string data = clip.GetContent();
            try
            {
                HeHEAD head = JsonUtility.FromJson<HeHEAD>(data);
                if (head.Metadata.Version == 1)
                {
                    HeFormat10 obj = JsonUtility.FromJson<HeFormat10>(data);
                    Event last = obj.Pattern[obj.Pattern.Count - 1].Event;
                    if (last.Type.Equals("continuous"))
                    {
                        duration = last.RelativeTime + last.Duration;
                    }
                    else if (last.Type.Equals("transient"))
                    {
                        duration = last.RelativeTime + CLIP_TRANSIENT_DURATION;
                    }
                    else
                    {
                        Debug.LogError("RichTap Haptics: Invalid HE1.0 event type.");
                    }
                }
                else if (head.Metadata.Version == 2)
                {
                    HeFormat20 obj = JsonUtility.FromJson<HeFormat20>(data);
                    PatternListItem items = obj.PatternList[obj.PatternList.Count - 1];
                    int maxEventEndTime = 0;
                    foreach (PatternItem pattern in items.Pattern)
                    {
                        int endTime = 0;
                        if (pattern.Event.Type.Equals("continuous"))
                        {
                            endTime = pattern.Event.RelativeTime + pattern.Event.Duration;
                        }
                        else if (pattern.Event.Type.Equals("transient"))
                        {
                            endTime = pattern.Event.RelativeTime + CLIP_TRANSIENT_DURATION;
                        }
                        else
                        {
                            Debug.LogError("RichTap Haptics: Invalid HE2.0 event type.");
                            break;
                        }
                        if (endTime > maxEventEndTime)
                        {
                            maxEventEndTime = endTime;
                        }
                    }
                    duration = items.AbsoluteTime + maxEventEndTime;
                }
                else
                {
                    Debug.Log("RichTap Haptics: Invalied HE format.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception happened: {e.Message}");
            }
            return duration;
        }

    }

}
