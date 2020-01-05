namespace Cake.Issues.Reporting.Generic
{
    using System.Collections.Generic;
    using Cake.Core.Diagnostics;
    using Cake.Core.IO;

    /// <summary>
    /// Generator for creating text based issue reports.
    /// Is simply a wrapper for underlying report generation methods.
    /// </summary>
    internal class GenericIssueReportGenerator : IIssueReportFormat, IGenericIssueReportGenerator
    {
        private IGenericIssueReportGenerator generator;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIssueReportGenerator"/> class.
        /// </summary>
        /// <param name="log">The Cake log context.</param>
        /// <param name="settings">Settings for reading the log file.</param>
        public GenericIssueReportGenerator(ICakeLog log, GenericIssueReportFormatSettings settings)
        {
            settings.NotNull(nameof(settings));
#if NET461
            this.generator = new RazorEngineIssueReportGenerator(log, settings);
#endif
#if NETCOREAPP
            this.generator = new RazorLightIssueReportGenerator(log, settings);
#endif
            this.generator.Configure();
        }

        public void AssertInitialized()
        {
            this.generator.NotNull(nameof(generator));
        }

        public void Configure()
        {
            this.generator.NotNull(nameof(generator));
            this.generator.Configure();
        }

        public FilePath CreateReport(IEnumerable<IIssue> issues)
        {
            this.generator.NotNull(nameof(generator));
            return this.generator.CreateReport(issues);
        }

        public bool Initialize(CreateIssueReportSettings settings)
        {
            this.generator.NotNull(nameof(generator));
            return this.generator.Initialize(settings);
        }
    }
}
