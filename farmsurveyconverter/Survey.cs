using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace farmsurveyconverter
{
    /// <summary>
    /// Describes a single point in the survey
    /// </summary>
    internal class SurveyPoint
    {
        public double OffsetX;
        public double OffsetY;
        public double Height;
        public string Name;

        public SurveyPoint
            (
            )
        {
            OffsetX = 0;
            OffsetY = 0;
            Height = 0;
            Name = null;
        }

        public SurveyPoint
            (
            double OffsetX,
            double OffsetY,
            double Height
            ) : this(OffsetX, OffsetY, Height, null)
        {
        }

        public SurveyPoint
            (
            double OffsetX,
            double OffsetY,
            double Height,
            string Name
            )
        {
            this.OffsetX = OffsetX;
            this.OffsetY = OffsetY;
            this.Height = Height;
            this.Name = Name;
        }

        public override string ToString()
        {
            if (Name != null)
                return string.Format("{0},{1},{2} [{3}]", OffsetX, OffsetY, Height, Name);
            else
                return string.Format("{0},{1},{2}", OffsetX, OffsetY, Height);
        }
    }

    internal class Survey
    {
        public double BaseLatitude;
        public double BaseLongitude;
        public List<SurveyPoint> Points = new List<SurveyPoint>();
    }
}
