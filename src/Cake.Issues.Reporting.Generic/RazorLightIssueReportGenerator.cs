#if NETCOREAPP
namespace Cake.Issues.Reporting.Generic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;
    using RazorLight;

    /// <summary>
    /// Generator for creating text based issue reports.
    /// </summary>
    internal class RazorLightIssueReportGenerator : IssueReportFormat, IGenericIssueReportGenerator
    {
        private readonly GenericIssueReportFormatSettings genericIssueReportFormatSettings;
        private IRazorLightEngine engine;
        private string[] defaultNamespaces = new string[] { "System.Linq", "System.Collections.Generic" };

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
                var defaultoptions = this.genericIssueReportFormatSettings.Options;
                var options = ViewBagHelper.ParsePropertiesFromTemplate(this.genericIssueReportFormatSettings.Template, defaultoptions);

                var result = this.engine.CompileRenderStringAsync(
                    Guid.NewGuid().ToString(),
                    this.genericIssueReportFormatSettings.Template,
                    issues,
                    options.ToExpandoObject())
                    .Result;
                File.WriteAllText(this.Settings.OutputFilePath.FullPath, result);

                return this.Settings.OutputFilePath;
            }
            catch (Exception e)
            {
                this.Log.Error(e.Message);

                throw;
            }
        }

        private void ConfigureRazorLight()
        {
            this.engine = new RazorLightEngineBuilder()
                 .AddDefaultNamespaces(this.defaultNamespaces)
                 .UseEmbeddedResourcesProject(typeof(RazorLightIssueReportGenerator))
                 .UseMemoryCachingProvider()
                 .Build();
        }
    }
}
#endif