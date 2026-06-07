using LogRegister.Interfaces;

namespace SMWHubLogging.LoggingRegisters;

public sealed class DataUsage : ILoggingEntry
{
    public bool AppearWithoutVerbose => true;
	public bool AppearInErrors => false;
    public string MessageTypeKey => LogMessageTypeKeys.DATA_USAGE;
    public IReadOnlyDictionary<string, string> Parameters { get; }
    public DataUsage(string name, long size)
		{
			float banks = size / (32 * 1024f);
			float mb = size / (1024 * 1024f);
			Parameters = new Dictionary<string, string>
			{
				{ "name", $"'{name}'" },
				{ "size", $"{mb:0.00}" },
				{ "banks", $"{banks:0.00}" }
			};
		}
	}
