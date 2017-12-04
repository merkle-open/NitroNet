using System;
using System.Collections.Generic;
using System.Configuration;
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
            var resultPage = BuilderModelResources.index;
            if (_modelBuilder.GenerateSingleFile)
            {
                resultPage = resultPage.Replace("cntr", "cntr hidden");
            }
            return resultPage;
            
        }

        public string Generate()
        {
            var overrideClasses = CheckIfTrue(Request.QueryString["override"]);
            var resultPage = BuilderModelResources.index;

            // Activate Console
            resultPage = resultPage.Replace("console hidden", "console");
            if(_modelBuilder.GenerateSingleFile)
            {
                resultPage = resultPage.Replace("cntr", "cntr hidden");
            }

            try
            {
                var result = _modelBuilder.GenerateModels(overrideClasses);
                var outputConsole = new StringBuilder();

                outputConsole.Append($"NitroNet Modelbuilder took {result.GenerationTime.TotalMilliseconds}ms to generate {result.Models.Count()} file(s)");
                foreach (var item in result.Models)
                {
                    outputConsole.Append($"<br />----<br />Generates <strong>{item.Name}</strong> with a character count of {item.Size}");
                }
                resultPage = resultPage.Replace("{{Result}}", outputConsole.ToString());
            }
            catch (Exception ex)
            {
                resultPage = resultPage.Replace("successMessage", "errorMessage");
                resultPage = resultPage.Replace("{{Result}}", ex.Message);
            }

            return resultPage;
        }

        private bool CheckIfTrue(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Equals("true", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
