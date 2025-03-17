using System.Text;
using System.Text.RegularExpressions;

namespace TBAT2CS;
public partial class Parser
{
    public Parser(Func<string, string> typeCallback, Func<string, string> nameCallback)
    {
        _typeCallback = typeCallback;
        _nameCallback = nameCallback;
    }

    private Func<string, string> _typeCallback;
    private Func<string, string> _nameCallback;

    public string Parse(string input)
    {
        var (header, content) = SplitHeaderAndBody(input);
        var sb = new StringBuilder();
        sb.Append(ParseHeader(header));
        sb.Append(ParseContent(content));
        return sb.ToString();
    }

    public string Parse(string header, string content)
    {
        var sb = new StringBuilder();
        sb.Append(ParseHeader(header));
        sb.Append(ParseContent(content));
        return sb.ToString();
    }

    private string ParseHeader(string input)
    {
        var r = ParseHeaderRegex().Match(input);
        var className = r.Groups["cn"].Value;
        var description = r.Groups["cd"].Value;

        var sb = new StringBuilder();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// {description.Trim('\r', '\n')}");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public class {className}");

        return sb.ToString();
    }

    private string ParseContent(string input)
    {
        var sb = new StringBuilder();
        sb.AppendLine("{");
        foreach (Match math in ParseContentRegex().Matches(input))
        {
            var jsonPropertyName = math.Groups["pn"].Value;
            var typeName = math.Groups["tn"].Value;
            var description = math.Groups["d"].Value;
            GenerateProperty(sb, jsonPropertyName, typeName, description);
        }
        sb.AppendLine("}");

        return sb.ToString();
    }

    private (string header, string content) SplitHeaderAndBody(string input)
    {
        var r = SplitHeaderAndBodyRegex().Match(input);
        var header = r.Groups["h"].Value;
        var body = r.Groups["b"].Value;
        return (header, body);
    }

    private void GenerateProperty(StringBuilder sb, string json, string type, string description)
    {
        sb.AppendLine("\t/// <summary>");
        sb.AppendLine($"\t/// {description.Trim('\r', '\n')}");
        sb.AppendLine("\t/// </summary>");
        sb.AppendLine($"\t[JsonPropertyName(\"{json}\")]");
        sb.AppendLine($"\tpublic {_typeCallback.Invoke(type)} {_nameCallback.Invoke(json)} {{ get; set; }}");
        sb.AppendLine();
    }


    [GeneratedRegex(@"(?<cn>[A-z]+)[\r\n]+?(?<cd>.+)")]
    private static partial Regex ParseHeaderRegex();

    [GeneratedRegex(@"^(?<pn>[a-z_]+?)[ \r\t]+?(?<tn>Array of .+?|[A-z]+)[ \r\t]+?(?<d>.+)$", RegexOptions.Multiline)]
    private static partial Regex ParseContentRegex();

    [GeneratedRegex(@"^(?<h>.+)[\n\r]+?Field[ \t\r]+?Type[ \t\r]+?Description[ \n\r]+?(?<b>.+)$", RegexOptions.Multiline | RegexOptions.Singleline)]
    private static partial Regex SplitHeaderAndBodyRegex();
}
