using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace farmsurveyconverter
{
    internal class HeightScalingProcessor : IProcessor
    {
        public double ScaleFactor = 3.0;

        public void Process
            (
            Survey Surv
            )
        {
            foreach (SurveyPoint Point in Surv.Points)
            {
                Point.Height = Point.Height * ScaleFactor;
            }
        }
    }
}
