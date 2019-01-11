using System;
using HandlebarsDotNet;
using NitroNet.ViewEngine.ViewEngines.HandlebarsNet;
using System.Collections.Generic;

namespace NitroNet.ViewEngine.TemplateHandler.HandlebarsNet
{
    public class HandlebarsNetHelperHandlerFactory : IHandlebarsNetHelperHandlerFactory
    {
        private readonly INitroTemplateHandlerFactory _nitroTemplateHandlerFactory;

        public HandlebarsNetHelperHandlerFactory(INitroTemplateHandlerFactory nitroTemplateHandlerFactory)
        {
            _nitroTemplateHandlerFactory = nitroTemplateHandlerFactory;
        }

        public List<KeyValuePair<string, HandlebarsHelper>> Create()
        {
            var helperDictionary = new List<KeyValuePair<string, HandlebarsHelper>>();

            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("pattern", new HandlebarsNetComponentHandler(_nitroTemplateHandlerFactory.Create()).Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("component", new HandlebarsNetComponentHandler(_nitroTemplateHandlerFactory.Create()).Evaluate));
            //TODO: Verhalten wie ">"
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("partial", new HandlebarsNetPartialHandler(_nitroTemplateHandlerFactory.Create()).Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("placeholder", new HandlebarsNetPlaceholderHandler(_nitroTemplateHandlerFactory.Create()).Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("t", new HandlebarsNetLabelHandler(_nitroTemplateHandlerFactory.Create()).Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("template-id", new HandlebarsNetTemplateIdHandler().Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("grid-width", new HandlebarsNetGridWidthHandler().Evaluate));
            helperDictionary.Add(new KeyValuePair<string, HandlebarsHelper>("grid-component-width", new HandlebarsNetGridComponentWidthHandler().Evaluate));

            return helperDictionary;
        }

        public List<KeyValuePair<string, HandlebarsBlockHelper>> CreateForBlocks()
        {
            var helperDictionary = new List<KeyValuePair<string, HandlebarsBlockHelper>>();

            helperDictionary.Add(new KeyValuePair<string, HandlebarsBlockHelper>("grid-cell", new HandlebarsNetGridCellHandler().Evaluate));

            return helperDictionary;
        }
    }
}
