using Microsoft.Extensions.Configuration;
using Octokit;

namespace BlOrders2023.Services;
public class IssueSubmitter
{

    private readonly GitHubClient client = new(new ProductHeaderValue("big-turkey-app"));
    private readonly long RepoID = 567698611;
    public async void SubmitIssueAsync(string title, string description, IEnumerable<string> tags = null)
    {
        var issue = new NewIssue(title)
        {
            Body = $"{description}",
        };
        foreach(var tag in tags)
        {
            issue.Labels.Add(tag);
        }

        var config = App.GetService<IConfiguration>();
        client.Credentials = new Credentials(config["APIKeys:GitHub"]);
        await client.Issue.Create(RepoID, issue);
    }
}
