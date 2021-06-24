using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace farmsurveyconverter
{
    internal interface IProcessor
    {
        void Process(Survey Surv);
    }
}
