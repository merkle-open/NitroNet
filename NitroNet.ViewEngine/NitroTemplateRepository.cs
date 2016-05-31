using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;

namespace NitroNet.ViewEngine
{
	public class NitroTemplateRepository : ITemplateRepository, IDisposable
	{
		private readonly IFileSystem _fileSystem;
        private readonly INitroNetConfig _configuration;
		private Dictionary<string, FileTemplateInfo> _templates;
        private Dictionary<string, List<FileTemplateInfo>> _templatesByName;
        private IEnumerable<IDisposable> _viewSubscriptions;
        private IEnumerable<IDisposable> _partialSubscriptions;
        private IEnumerable<IDisposable> _componentSubscriptions;

        public NitroTemplateRepository(IFileSystem fileSystem, INitroNetConfig configuration)
		{
			_fileSystem = fileSystem;
            _configuration = configuration;

			InitCache();

			if (!_fileSystem.SupportsSubscribe)
				return;

            var componentSubscriptions = new List<IDisposable>();
            foreach (var componentPath in _configuration.ComponentPaths)
            {
                componentSubscriptions.Add(
                    _fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(componentPath.ToString()),
                        "html", files => InitCache()));
            }

            _componentSubscriptions = componentSubscriptions;

            var partialSubscriptions = new List<IDisposable>();
            foreach (var partialPath in _configuration.PartialPaths)
            {
                partialSubscriptions.Add(
                    _fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(partialPath.ToString()),
                        "html", files => InitCache()));
            }

            _partialSubscriptions = partialSubscriptions;

            var viewSubscriptions = new List<IDisposable>();
            foreach (var viewPath in _configuration.ViewPaths)
            {
                viewSubscriptions.Add(
                    _fileSystem.SubscribeDirectoryGetFilesAsync(PathInfo.Create(viewPath.ToString()),
                        "html", files => InitCache()));
            }

            _viewSubscriptions = viewSubscriptions;
		}

		private void InitCache()
		{
            List<FileTemplateInfo> viewTemplates = new List<FileTemplateInfo>();
            foreach (var viewPath in _configuration.ViewPaths)
            {
                viewTemplates.AddRange(
                    _fileSystem.DirectoryGetFiles(viewPath, "html").Select(f =>
                    {
                        var relativePath = GetTemplateId(f).RemoveStartSlash();
                        return new FileTemplateInfo(relativePath.ToString(), TemplateType.View, f, _fileSystem);
                    }));
            }

            List<FileTemplateInfo> partialTemplates = new List<FileTemplateInfo>();
            foreach (var partialPath in _configuration.PartialPaths)
            {
                partialTemplates.AddRange(
                    _fileSystem.DirectoryGetFiles(partialPath, "html").Select(f =>
                    {
                        var relativePath = GetTemplateId(f).RemoveStartSlash();
                        return new FileTemplateInfo(relativePath.ToString(), TemplateType.Partial, f, _fileSystem);
                    }));
            }

            List<FileTemplateInfo> componentTemplates = new List<FileTemplateInfo>();
		    foreach (var componentPath in _configuration.ComponentPaths)
		    {
		        componentTemplates.AddRange(
                    _fileSystem.DirectoryGetFiles(componentPath, "html").Select(f =>
                    {
                        var relativePath = GetTemplateId(f).RemoveStartSlash();
                        return new FileTemplateInfo(relativePath.ToString(), TemplateType.Component, f, _fileSystem);
                    }));
		    }

            _templates = componentTemplates
                .Concat(partialTemplates)
                .Concat(viewTemplates)
                .GroupBy(p => p.Id, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(i => i.Key, i => i.First(), StringComparer.InvariantCultureIgnoreCase);
            _templatesByName = new Dictionary<string, List<FileTemplateInfo>>();
            foreach (var template in _templates)
            {
                var templateName = template.Value.Name;
                if (_templatesByName.ContainsKey(templateName))
                {
                    _templatesByName[templateName].Add(template.Value);
                }
                else
                {
                    _templatesByName[templateName] = new List<FileTemplateInfo> { template.Value };
                }
		    }
		}

		public Task<TemplateInfo> GetTemplateAsync(string id)
		{
			FileTemplateInfo templateInfo;
			if (_templates.TryGetValue(id, out templateInfo))
				return Task.FromResult<TemplateInfo>(templateInfo);

            List<FileTemplateInfo> templateInfos;
		    if (_templatesByName.TryGetValue(id, out templateInfos))
		    {
		        var firstPartialFound = templateInfos.FirstOrDefault(t => t.Type.Equals(TemplateType.Partial));
		        if (firstPartialFound != null)
		        {
                    return Task.FromResult<TemplateInfo>(firstPartialFound);
                }

                return Task.FromResult<TemplateInfo>(templateInfos.FirstOrDefault());
            }

            return Task.FromResult<TemplateInfo>(null);
		}

	    public Task<IEnumerable<TemplateInfo>> GetTemplatesByNameAsync(string name)
	    {
            List<FileTemplateInfo> templateInfo;
            if (_templatesByName.TryGetValue(name, out templateInfo))
                 return Task.FromResult((IEnumerable<TemplateInfo>)templateInfo);

            return Task.FromResult((IEnumerable<TemplateInfo>)new List<TemplateInfo>());
        }

        private PathInfo GetTemplateId(PathInfo info)
        {
            return _fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(info), _fileSystem.Path.GetFileNameWithoutExtension(info));
        }

        public IEnumerable<TemplateInfo> GetAll()
		{
			return _templates.Values;
		}

        ~NitroTemplateRepository()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		    if (disposing)
		    {
		        foreach (var componentSubscription in _componentSubscriptions)
		        {
                    componentSubscription.Dispose();
                }
                foreach (var partialSubscription in _partialSubscriptions)
                {
                    partialSubscription.Dispose();
                }
                foreach (var viewSubscription in _viewSubscriptions)
                {
                    viewSubscription.Dispose();
                }
            }

            _componentSubscriptions = null;
		    _partialSubscriptions = null;
            _viewSubscriptions = null;
		}
	}
}