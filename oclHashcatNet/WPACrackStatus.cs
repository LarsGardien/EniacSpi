﻿using System.Collections.Generic;
using System.ComponentModel;

namespace oclHashcatNet.Extensions
{
    public enum WPAcrackCondition
    {
        Stopped,
        Paused, 
        Running,
        Aborted
    }

    public struct GPUStatus
    {
        public int Id { get; set; }
        public int Load { get; set; }
        public int Temperature { get; set; }
        public float Speed { get; set; }
    }

    public class WPACrackStatus
    {
        public WPACrackStatus()
        {
            this._GPUs = new List<GPUStatus>();
            this.condition = WPAcrackCondition.Stopped;
            this.progress = 0f;
        }

        private WPAcrackCondition condition;
        public WPAcrackCondition Condition { set { onPropertyChanged(this, new PropertyChangedEventArgs("GPUs")); condition = value; } get { return condition; } }

        private float progress;
        public float Progress{ get { return progress; } set { onPropertyChanged(this, new PropertyChangedEventArgs("GPUs")); progress = value; } }

        private IList<GPUStatus> _GPUs;
        public IList<GPUStatus> GPUs { get { return _GPUs; } set { onPropertyChanged(this, new PropertyChangedEventArgs("GPUs")); _GPUs = value; }}

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(object sender, PropertyChangedEventArgs ea)
        {
            this.PropertyChanged?.Invoke(sender, ea);
        }
    }
}