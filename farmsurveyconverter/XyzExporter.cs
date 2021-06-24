using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace farmsurveyconverter
{
    internal class XyzExporter : IExporter
    {
        /// <summary>
        /// Exports the file
        /// </summary>
        /// <param name="Surv">Survey to export</param>
        /// <param name="FileName">Path and name of file to export to</param>
        public void Export
            (
            Survey Surv,
            string FileName
            )
        {
            List<string> Out = new List<string>();

            foreach (SurveyPoint Point in Surv.Points)
            {
                Out.Add(string.Format("{0} {1} {2}", Point.OffsetX, Point.OffsetY, Point.Height));
            }

            File.WriteAllLines(FileName, Out);
        }
    }
}
