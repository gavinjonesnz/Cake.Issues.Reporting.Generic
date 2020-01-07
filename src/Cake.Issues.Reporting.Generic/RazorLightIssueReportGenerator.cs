#if NETCOREAPP
namespace Cake.Issues.Reporting.Generic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using RazorLight;
    using RazorLight.Compilation;

    /// <summary>
    /// Generator for creating text based issue reports.
    /// </summary>
    internal class RazorLightIssueReportGenerator : IssueReportFormat, IGenericIssueReportGenerator
    {
        private readonly GenericIssueReportFormatSettings genericIssueReportFormatSettings;
        private IRazorLightEngine engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorLightIssueReportGenerator"/> class.
        /// </summary>
        /// <param name="log">The Cake log context.</param>
        /// <param name="settings">Settings for reading the log file.</param>
        public RazorLightIssueReportGenerator(ICakeLog log, GenericIssueReportFormatSettings settings)
            : base(log)
        {
            settings.NotNull(nameof(settings));

            this.genericIssueReportFormatSettings = settings;

            this.ConfigureRazorLight();
        }

        public void Configure()
        {
            this.ConfigureRazorLight();
        }

        /// <inheritdoc />
        protected override FilePath InternalCreateReport(IEnumerable<IIssue> issues)
        {
            this.Log.Information("Creating report '{0}'", this.Settings.OutputFilePath.FullPath);

            try
            {
                var options = this.genericIssueReportFormatSettings.Options;
                var viewBag = new Dictionary<string, object>(GetDefaultTemplateOptions());

                // Override with options
                options.ToList().ForEach(x => viewBag[x.Key] = x.Value);

                var result = this.engine.CompileRenderStringAsync(
                    Guid.NewGuid().ToString(),
                    this.genericIssueReportFormatSettings.Template, 
                    issues,
                    viewBag.ToExpando()).Result;
                File.WriteAllText(this.Settings.OutputFilePath.FullPath, result);

                return this.Settings.OutputFilePath;
            }
            catch (Exception e)
            {
                this.Log.Error(e.Message);

                throw;
            }
        }

        // Hack to pull defaults for ViewBag from template
        // TODO: Is there a better option for this? 
        // Without it, currently templates with ViewBag will barf if not provided with all ViewBag props passed as options
        private IDictionary<string, object> GetDefaultTemplateOptions()
        {
            var options = new Dictionary<string, object>();
            // Manually for now
            options.Add("Title", null);
            options.Add("Theme", null);
            options.Add("ShowHeader", null);
            options.Add("EnableSearching", null);
            options.Add("EnableGrouping", null);
            options.Add("EnableFiltering", null);
            options.Add("EnableExporting", null);
            options.Add("ProviderTypeVisible", null);
            options.Add("ProviderTypeSortOder", null);
            options.Add("ProviderNameVisible", null);
            options.Add("ProviderNameSortOder", null);
            options.Add("PriorityVisible", null);
            options.Add("PrioritySortOrder", null);
            options.Add("PriorityNameVisible", null);
            options.Add("PriorityNameSortOrder", null);
            options.Add("ProjectPathVisible", null);
            options.Add("ProjectPathSortOder", null);
            options.Add("ProjectNameVisible", null);
            options.Add("ProjectNameSortOder", null);
            options.Add("FilePathVisible", null);
            options.Add("FilePathSortOder", null);
            options.Add("FileDirectoryVisible", null);
            options.Add("FileDirectorySortOder", null);
            options.Add("FileNameVisible", null);
            options.Add("FileNameSortOder", null);
            options.Add("LineVisible", null);
            options.Add("LineSortOder", null);
            options.Add("RuleVisible", null);
            options.Add("RuleSortOder", null);
            options.Add("RuleUrlVisible", null);
            options.Add("RuleUrlSortOder", null);
            options.Add("MessageVisible", null);
            options.Add("MessageSortOder", null);
            options.Add("GroupedColumns", null);
            options.Add("SortedColumns", null);
            options.Add("FileLinkSettings", null);
            options.Add("IdeIntegrationSettings", null);
            options.Add("AdditionalColumns", null);
            options.Add("JQueryLocation", null);
            options.Add("JQueryVersion", null);
            options.Add("JSZipLocation", null);
            options.Add("DevExtremeLocation", null);
            options.Add("DevExtremeVersion", null);

            //Regex r = new Regex(@"ViewBag\.{},", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            //var matches = r.Matches(this.genericIssueReportFormatSettings.Template);
            //foreach (Match m in matches)
            //{
            //    //this.genericIssueReportFormatSettings.Template
            //}
            return options;
        }

        private void ConfigureRazorLight()
        {
            IMetadataReferenceManager manager = new DefaultMetadataReferenceManager();
            engine = new RazorLightEngineBuilder()
                 .UseEmbeddedResourcesProject(typeof(RazorLightIssueReportGenerator))
                 //.AddMetadataReferences(MetadataResolver.GetMetadataReferences().ToArray())
                 .AddMetadataReferences(manager.AdditionalMetadataReferences.ToArray())
                 .UseMemoryCachingProvider()
                 .Build();
        }
    }
}
#endif