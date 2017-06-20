### New Features / Enhancements
- Greatly improved documentation:
	- Updated, corrected and extended
	- Improved structure
	- Better syntax highligthing of code examples
- Tests have been updated
- Partials are now working with the familiar syntax from handlebars: `{{> myPartial }}`
- Prereleases are now supported by the build routine and delivered via nuget.org if necessary
- The NuGet distribution was updated to version `3.5`
- The custom handlebars helper `{{pattern [...]}}` of *Nitro* is now supported
- NitroNet configuration:
	- It is now possible configure which file extensions *NitroNet* should respect
	- It is now possible to set filter regex' to exclude specific file paths from *NitroNet*
	- An example config is now delivered
- NitroNet now has the ability to read multi line handlebars helpers
- The assembly infos have been unified and updated
- ...and many code refactorings and small improvements

### Fixed Issues
- A specific path is now only watched by one file watcher (`NitroNet.ViewEngine.IO.FileSystem`)
- More robust resolving of the *DefaultTemplate* (`NitroNet.ViewEngine.DefaultComponentRepository`)
- `NitroNet.UnityModules` and `NitroNet.CastleWindsorModules` now reference the correct `NitroNet` dependency
- There are no longer exceptions thrown when frontend files are changed and *NitroNet* is running
- ...and many more small bug-fixes

### Removed Features / Breaking Changes
- NitroNet configuration:
	- The file was renamed from `config.json` to `nitronet-config.json`
	- The initial example config has to be renamed from `nitronet-config.json.example` to `nitronet-config.json` to make *NitroNet* work

### Update/Installation Instructions

TODO