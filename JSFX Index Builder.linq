<Query Kind="Statements">
  <Namespace>System.Xml.Serialization</Namespace>
</Query>

var serializer = new XmlSerializer(typeof(Index));

var index = new Index() { Name = "Mark Heath" };
var airwindows = new Category() { Name="Airwindows" };

var effect = new ReaPack() { Name = "Airwindows Bright Ambience 3", Desc = "Reverb" };
effect.Versions.Add(new Version()
{
	Name = "0.2",
	ChangeLog = "improved bit shifting for random number",
	Source = new Source("airwindows-brightambience3.jsfx"),
	Time = "2022-05-06T21:46:56+01:00"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows Console7", Desc = "Saturation" };
effect.Versions.Add(new Version()
{
	Name = "0.1",
	ChangeLog = "initial port from GitHub commit 558b93e",
	Source = new Source("airwindows-console7.jsfx"),
	Time = "2022-05-02"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows Galactic", Desc = "Reverb" };
effect.Versions.Add(new Version()
{
	Name = "0.1",
	ChangeLog = "initial port from GitHub commit 558b93e",
	Source = new Source("airwindows-galactic.jsfx"),
	Time = "2022-05-02"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows PurestAir", Desc = "EQ" };
effect.Versions.Add(new Version()
{
	Name = "0.1",
	ChangeLog = "initial port from GitHub commit 558b93e",
	Source = new Source("airwindows-purest-air.jsfx"),
	Time = "2022-05-02"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows PurestGain", Desc = "Gain" };
effect.Versions.Add(new Version()
{
	Name = "0.1",
	ChangeLog = "initial port from GitHub commit 558b93e",
	Source = new Source("airwindows-purest-gain.jsfx"),
	Time = "2022-05-02"
});
airwindows.ReaPacks.Add(effect);


effect = new ReaPack() { Name = "Airwindows NC-17", Desc = "Loudness" };
effect.Versions.Add(new Version()
{
	Name = "0.2",
	ChangeLog = "improved bit shifting for random number",
	Source = new Source("airwindows-nc-17.jsfx"),
	Time = "2022-05-06"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows ToTape6", Desc = "Tape" };
effect.Versions.Add(new Version()
{
	Name = "0.3",
	ChangeLog = "improved bit shifting for random number",
	Source = new Source("airwindows-totape6.jsfx"),
	Time = "2022-05-06"
});
airwindows.ReaPacks.Add(effect);

effect = new ReaPack() { Name = "Airwindows Verbity", Desc = "Reverb" };
effect.Versions.Add(new Version()
{
	Name = "0.2",
	ChangeLog = "improved bit shifting for random number",
	Source = new Source("airwindows-verbity.jsfx"),
	Time = "2022-05-06"
});
airwindows.ReaPacks.Add(effect);



index.Categories.Add(airwindows);

await using var memoryStream = new MemoryStream();
XmlTextWriter streamWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
streamWriter.Formatting = Formatting.Indented;

serializer.Serialize(streamWriter, index);

var result = Encoding.UTF8.GetString(memoryStream.ToArray());
result.Dump();


[XmlRoot("index")] public class Index {
	[XmlAttribute("version")] public int Version { get; } = 1;
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlElement("category")] public List<Category> Categories { get; } = new List<Category>();
}

[XmlRoot("category")] public class Category {
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlElement("reapack")] public List<ReaPack> ReaPacks { get; } = new List<ReaPack>();
}

public class ReaPack {
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlAttribute("type")] public string Type { get; } = "effect";
	[XmlAttribute("desc")] public string Desc { get; set; }
	[XmlElement("version")] public List<Version> Versions { get; } = new List<Version>();
}

public class Version {
	[XmlAttribute("name")] public string Name { get; set; }
	[XmlAttribute("author")] public string Author { get; } = "Mark Heath";
	[XmlAttribute("time")] public string Time { get; set; }
	[XmlElement("changelog")]public string ChangeLog { get; set; } // might need to use CData
	[XmlElement("source")] public Source Source { get; set; } // n.b. can have multiple files
}

public class Source
{
	private Source() {} // needed
	public Source(string file) { File = file; Value = $"https://raw.githubusercontent.com/markheath/jsfx/main/{file}"; }
	[XmlAttribute("file")] public string File { get; set; }
	[XmlText] public string Value { get; set; }
}