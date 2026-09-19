using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.RecordSystem
{
    public struct InputFrame
    {
        private Vector2 Move { set; get; }
        private bool Jump { set; get; }
    }
    
    public interface IRecorder
    {
        void StartRecord(out List<InputFrame> inputFrames, InputFrame currentFrame, float time);
    }
    
    public class Recorder : IRecorder
    {
        public void StartRecord(out List<InputFrame> inputFrames,, float time)
        {
            inputFrames = new List<InputFrame>();
            
        }
    }
}
