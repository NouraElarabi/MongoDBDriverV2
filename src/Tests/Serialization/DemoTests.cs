namespace Tests.Serialization
{
	using MongoDB.Bson.IO;

	public class DemoTests
	{
		public DemoTests()
		{
			JsonWriterSettings.Defaults.Indent = true;
		}
	}
}