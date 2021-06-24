using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace farmsurveyconverter
{
    internal interface IExporter
    {
        void Export(Survey Surv, string FileName);
    }
}
