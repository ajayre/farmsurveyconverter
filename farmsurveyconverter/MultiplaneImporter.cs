using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace farmsurveyconverter
{
    /// <summary>
    /// Imports multiplane files
    /// </summary>
    internal class MultiplaneImporter : IImporter
    {
        /// <summary>
        /// Imports a file
        /// </summary>
        /// <param name="FileName">Path and name of file to import</param>
        /// <returns>Imported survey data</returns>
        public Survey Import
            (
            string FileName
            )
        {
            Survey Surv = new Survey();

            string[] Lines = File.ReadAllLines(FileName);
            if (Lines.Length == 0) throw new Exception("Input file is empty or not a text file");

            // process first line
            string[] BaseParts = Lines[0].Split(new char[] { '\t' });
            if (BaseParts.Length != 6) throw new Exception(string.Format("Expected six parts in first line, found {0} parts", BaseParts.Length));

            double BaseOffsetX = double.Parse(BaseParts[1]);
            double BaseOffsetY = double.Parse(BaseParts[2]);
            double BaseHeight = double.Parse(BaseParts[3]);

            Surv.Points.Add(new SurveyPoint(BaseOffsetX, BaseOffsetY, 0));

            string[] LocParts = BaseParts[4].Split(new char[] { ' ' });
            if (LocParts.Length != 4) throw new Exception(string.Format("Expected four parts in location text, found {0} parts", LocParts.Length));
            Surv.BaseLatitude = ParseLatitude(LocParts[1]);
            Surv.BaseLongitude = ParseLongitude(LocParts[3]);

            if (Lines.Length < 2) throw new Exception("No survey points found");

            // parse lines
            for (int li = 1; li < Lines.Length; li++)
            {
                string[] PointParts = Lines[li].Split(new char[] { '\t' });
                if (PointParts.Length < 4) throw new Exception(string.Format("Unable to parse line {0}, not enough parts", li + 1));

                double X = double.Parse(PointParts[1]);
                double Y = double.Parse(PointParts[2]);
                double Z = double.Parse(PointParts[3]);

                string Name = null;
                if (PointParts.Length > 4)
                {
                    Name = PointParts[4].Trim();
                }

                Surv.Points.Add(new SurveyPoint(X, Y, Z - BaseHeight, Name));
            }

            Console.WriteLine(string.Format("{0} survey points found", Surv.Points.Count));

            return Surv;
        }

        /// <summary>
        /// Parses text containing latitude in the format "N36:26:51.619"
        /// </summary>
        /// <param name="Text">Text to parse</param>
        /// <returns>Latitude</returns>
        private double ParseLatitude
            (
            string Text
            )
        {
            double Lat = 0;

            // fixme - to do
            
            return Lat;
        }

        /// <summary>
        /// Parses text containing longitude in the format "W36:26:51.619"
        /// </summary>
        /// <param name="Text">Text to parse</param>
        /// <returns>Longitude</returns>
        private double ParseLongitude
            (
            string Text
            )
        {
            double Lon = 0;

            // fixme - to do

            return Lon;
        }
    }
}
