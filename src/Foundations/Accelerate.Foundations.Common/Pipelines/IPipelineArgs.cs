﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Pipelines
{
    public interface IPipelineArgs<T>
    {
        public dynamic Values { get; set; } 
        public T Value { get; set; }
    }
}
