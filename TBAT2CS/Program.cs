using System.Text;
using TBAT2CS;

var parser = new Parser(
t =>
{
    var isArray = false;
    if (t.Length >= 8 && t[..8] == "Array of")
    {
        isArray = true;
        t = t.Split(' ').Last();
    }
    var transformType = t switch
    {
        "Boolean" => "bool",
        "True" => "bool",
        "String" => "string",
        "Integer" => "long",
        _ => t,
    };
    return isArray ? transformType + "[]" : transformType;
},
n =>
{
    var sb = new StringBuilder();
    foreach (var item in n.Split('_'))
    {
        sb.Append(item.First().ToString().ToUpper());
        sb.Append(item[1..]);
    }

    return sb.ToString();
});


Console.ReadLine();
