<Query Kind="Statements">
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

var serializer = new XmlSerializer(typeof(Index));

var index = new Index() { Name = "Mark Heath" };
var airwindows = new Category() { Name="Airwindows" };
var noteMaps = new Category() { Name="EZDrummer MIDI Note Maps" };
var folder = Path.GetDirectoryName(Util.CurrentQueryPath);

void AddEffect(string file)
{
	var lines = File.ReadAllLines(file);
	var desc = lines.First(l => l.StartsWith("desc:")).Substring(5).Replace("(Mark Heath)","").Trim();
	var regex = new Regex(@"\/\/ (\d\.\d) (\d+ [a-zA-Z]+ \d{4}) \- (.*)");
	var versionLine = lines.Last(l => regex.IsMatch(l));
	var matches = regex.Match(versionLine);
	var version = matches.Groups[1].Value;
	var date = DateTime.Parse(matches.Groups[2].Value);
	var message = matches.Groups[3].Value;
	var effect = new ReaPack() { Name = desc };
	effect.Versions.Add(new Version()
	{
		Name = version,
		ChangeLog = message,
		Source = new Source(Path.GetFileName(file)),
		Time = date.ToString("yyyy-MM-dd\\THH:mm:ss+01:00")
	});
	airwindows.ReaPacks.Add(effect);
}

void AddNoteMap(string file, string subfolder)
{
	var lines = File.ReadAllLines(file);
	var desc =  Path.GetFileNameWithoutExtension(file);
	var regex = new Regex(@"\# v(\d\.\d) (\d+ [a-zA-Z]+ \d{4})");
	var versionLine = lines.Last(l => regex.IsMatch(l));
	var matches = regex.Match(versionLine);
	var version = matches.Groups[1].Value;
	var date = DateTime.Parse(matches.Groups[2].Value);
	var message = matches.Groups[3].Value;
	var effect = new ReaPack() { Name = desc, Type="midinotenames" };
	effect.Versions.Add(new Version()
	{
		Name = version,
		ChangeLog = message,
		Source = new Source(Path.GetFileName(file), subfolder),
		Time = date.ToString("yyyy-MM-dd\\THH:mm:ss+01:00")
	});
	noteMaps.ReaPacks.Add(effect);
}

foreach (var f in Directory.GetFiles(folder, "airwindows-*.jsfx"))
{
	AddEffect(f);
}
foreach(var f in Directory.GetFiles(Path.Combine(folder,"note-names"), "*.txt"))
{
	AddNoteMap(f, "note-names/");
}
	

index.Categories.Add(airwindows);
index.Categories.Add(noteMaps);

await using var memoryStream = new MemoryStream();
XmlTextWriter streamWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
streamWriter.Formatting = Formatting.Indented;

serializer.Serialize(streamWriter, index);

var result = Encoding.UTF8.GetString(memoryStream.ToArray());
result.Dump();


[XmlRoot("index")] public class Index {
	[XmlAttribute("version")] public int Version { get; set; } = 1;
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlElement("category")] public List<Category> Categories { get; } = new List<Category>();
}

[XmlRoot("category")] public class Category {
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlElement("reapack")] public List<ReaPack> ReaPacks { get; } = new List<ReaPack>();
}

public class ReaPack {
	[XmlAttribute("name")] public string Name { get; set; } // if there is a desc attribute, that is shown in ReaPack UI instead, we'll use name
	[XmlAttribute("type")] public string Type { get; set; } = "effect";	
	[XmlElement("version")] public List<Version> Versions { get; } = new List<Version>();
}

public class Version {
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlAttribute("author")] public string Author { get; set; } = "Mark Heath";
	[XmlAttribute("time")] public string Time { get; set; }
	[XmlElement("changelog")]public string ChangeLog { get; set; } // might need to use CData
	[XmlElement("source")] public Source Source { get; set; } // n.b. can have multiple files
}

public class Source
{
	private Source() { } // needed for serializer
	public Source(string file, string subfolder = "") 
	{ 
		File = file; 
		Value = $"https://raw.githubusercontent.com/markheath/jsfx/main/{subfolder}{file.Replace(" ","%20")}"; }
	[XmlAttribute("file")] public string File { get; set; }
	[XmlText] public string Value { get; set; }
}