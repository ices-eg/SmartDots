using System.Collections.Generic;
using System.Text.RegularExpressions;

public struct LinkItem
{
    public string Href;
    public string Text;

    public override string ToString()
    {
        return Href + "\n\t" + Text;
    }
}

static class LinkFinder
{
    public static List<LinkItem> Find(string file)
    {
        List<LinkItem> list = new List<LinkItem>();

        // 1.
        // Find all matches in file.
        MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
            RegexOptions.IgnoreCase);

        // 2.
        // Loop over each match.
        foreach (Match m in m1)
        {
            string value = m.Groups[1].Value;
            LinkItem i = new LinkItem();

            // 3.
            // Get href attribute.
            Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                i.Href = m2.Groups[1].Value;
            }

            // 4.
            // Remove inner tags from text.
            string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                RegexOptions.IgnoreCase);
            i.Text = t;

            list.Add(i);
        }
        return list;
    }
}
