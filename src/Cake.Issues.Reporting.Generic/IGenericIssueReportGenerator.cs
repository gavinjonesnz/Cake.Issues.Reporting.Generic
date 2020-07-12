namespace Cake.Issues.Reporting.Generic
{
    using System.Collections.Generic;
    using Cake.Core.IO;

    internal interface IGenericIssueReportGenerator
    {
        void Configure();
        FilePath CreateReport(IEnumerable<IIssue> issues);
        bool Initialize(CreateIssueReportSettings settings);
    }
}