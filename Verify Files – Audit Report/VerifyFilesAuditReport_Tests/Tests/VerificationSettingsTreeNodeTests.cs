using VerifyFilesAuditReport.Components.SettingsProvider.Model;

namespace VerifyFilesAuditReport_Tests.Tests;

public class VerificationSettingsTreeNodeTests
{
    private static VerificationSettingsTreeNode BuildTree() =>
        new()
        {
            Name = "Root",
            Values =
            [
                new() { Name = "ChildA", Value = "1" },
                new()
                {
                    Name = "ChildB",
                    Values = [new() { Name = "GrandChild", Value = "42" }]
                }
            ]
        };

    [Fact]
    public void FindSettingValueRecursive_FindsSelf()
    {
        var tree = BuildTree();

        Assert.Same(tree, tree.FindSettingValueRecursive("Root"));
    }

    [Fact]
    public void FindSettingValueRecursive_FindsDirectChild()
    {
        var tree = BuildTree();

        Assert.Equal("1", tree.FindSettingValueRecursive("ChildA")?.Value);
    }

    [Fact]
    public void FindSettingValueRecursive_FindsNestedNode()
    {
        var tree = BuildTree();

        Assert.Equal("42", tree.FindSettingValueRecursive("GrandChild")?.Value);
    }

    [Fact]
    public void FindSettingValueRecursive_ReturnsNullWhenMissing()
    {
        var tree = BuildTree();

        Assert.Null(tree.FindSettingValueRecursive("Nope"));
    }
}
