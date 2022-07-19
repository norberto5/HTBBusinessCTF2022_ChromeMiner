using System.IO;

var allText = File.ReadAllText("deobfuscated.js");

var part1Text = allText.Substring(0, allText.LastIndexOf("q ="));
List<string> q = LoadArray(part1Text);
part1Text = SanitizeText(part1Text, q);

var part2Text = allText.Substring(allText.LastIndexOf("q ="));
List<string> q2 = LoadArray(part2Text);
part2Text = SanitizeText(part2Text, q2);

File.WriteAllText("sanitized.js", part1Text+part2Text);

private string SanitizeText(string text, List<string> dictionary)
{
    for (int i =0; i < dictionary.Count; i++)
    {
        if(string.IsNullOrEmpty(dictionary[i]))
        {
            text = text.Replace($" + q[{i}]", "");
            text = text.Replace($"q[{i}] + ", "");
            text = text.Replace($"q[{i}]", "");
        }
        else
        {
            text = text.Replace($"q[{i}]", $"\"{dictionary[i]}\"");
        }
    }

    // for some reason, the deobfuscator I used didn't replace 1e3 with base 10 integer, but 1e3 (483) is empty anyway.
    text = text.Replace($" + q[1e3]", "");
    text = text.Replace($"q[1e3] + ", "");
    text = text.Replace("q[1e3]", ""); 

    text = text.Replace("\" + \"", ""); // concatenate words
    text = text.Remove(text.IndexOf("q ="), text.IndexOf("\r", text.IndexOf("q =")) - text.IndexOf("q =")+2); // remove line with q = ...
    return text;
}

private List<string> LoadArray(string text)
{
    var lineOfDictionary = text.Substring(text.IndexOf("q ="), text.IndexOf("\r", text.IndexOf("q =")) - text.IndexOf("q =")+2);
    var q = new List<string>();
    while(true)
    {
        var firstQuote = lineOfDictionary.IndexOf('"');
        var secondQuote = lineOfDictionary.IndexOf('"', firstQuote+1);

        if(firstQuote == -1)
        {
            break;
        }

        var tmp = lineOfDictionary.Substring(firstQuote+1, secondQuote-firstQuote-1);
        q.Add(tmp);
        lineOfDictionary = lineOfDictionary.Remove(0, secondQuote+1);
    }
    return q;
}