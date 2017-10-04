using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NitroNet.ModelBuilder
{
    public class ModelBuilderController : Controller
    {
        private readonly IModelBuilder _modelBuilder;
        public ModelBuilderController(IModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public string Index()
        {
            return BuilderModelResources.index;
            //return System.Text.Encoding.UTF8.GetString(file);
            //return "TestService: " + _modelBuilder.ToString();
        }

        public string Generate()
        {
            var overrideAll = Request.QueryString["override"];
            _modelBuilder.GenerateModels();
            return "TestService: " + _modelBuilder.ToString();
        }
    }
}
