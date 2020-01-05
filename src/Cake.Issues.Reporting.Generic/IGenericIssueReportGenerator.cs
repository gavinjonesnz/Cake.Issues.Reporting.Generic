using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.Issues.Reporting.Generic
{
    internal interface IGenericIssueReportGenerator
    {
        void Configure();
        FilePath CreateReport(IEnumerable<IIssue> issues);
        bool Initialize(CreateIssueReportSettings settings);
    }
}