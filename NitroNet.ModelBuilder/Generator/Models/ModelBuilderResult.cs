using System;
using System.Collections.Generic;

namespace NitroNet.ModelBuilder.Generator.Models
{
    public class ModelBuilderResult
    {
        public TimeSpan GenerationTime { get; set; }
        public IEnumerable<ModelBuilderItem> Models { get; set; }

        public ModelBuilderResult(IEnumerable<ModelBuilderItem> models)
        {
            Models = models;
        }
    }
}
