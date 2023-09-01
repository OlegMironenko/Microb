using FluentAssertions;
using Rozum.BrainTrainer.Identity.Api.Extensions;
using Xunit;

namespace Rozum.BrainTrainer.Identity.Api.UnitTests.Extensions;

public class StringExtensionsToSnakeCase
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("TableName", "table_name")]
    [InlineData("tableName", "table_name")]
    [InlineData("table_Name", "table_name")]
    [InlineData("table_name", "table_name")]
    [InlineData("LongTableName_WithUnderscore", "long_table_name_with_underscore")]
    public void Test(string input, string expectedResult)
    {
        expectedResult.Should().Be(input.ToSnakeCase());
    }
}
