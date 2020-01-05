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
            Regex r = new Regex(@"ViewBag\.{},", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            var matches = r.Matches(this.genericIssueReportFormatSettings.Template);
            foreach (Match m in matches)
            {
                //this.genericIssueReportFormatSettings.Template
            }
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