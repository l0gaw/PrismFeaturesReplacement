namespace PrismFeaturesReplacement.Models;

//A little modified copy of NavigationParameters from the Prism Library(https://github.com/PrismLibrary/Prism)
//They've been used for migration reasons
public interface INavigationParameters
{
    void Add(string key, object value);
    bool ContainsKey(string key);
    int Count { get; }
    IEnumerable<string> Keys { get; }
    T GetValue<T>(string key);
    IEnumerable<T> GetValues<T>(string key);
    bool TryGetValue<T>(string key, out T value);
    object this[string key] { get; }
} 

public class NavigationParameters : INavigationParameters, IEnumerable<KeyValuePair<string, object>>
{
    private readonly List<KeyValuePair<string, object>> entries = new List<KeyValuePair<string, object>>();

    public object this[string key]
    {
        get
        {
            foreach (var entry in entries)
            {
                if (string.Compare(entry.Key, key, StringComparison.Ordinal) == 0)
                {
                    return entry.Value;
                }
            }

            return null;
        }
    }

    public int Count => entries.Count;

    public IEnumerable<string> Keys =>
        entries.Select(x => x.Key);

    public void Add(string key, object value) =>
        entries.Add(new KeyValuePair<string, object>(key, value));

    public bool ContainsKey(string key) => entries.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() =>
        entries.GetEnumerator();

    public T GetValue<T>(string key) => entries.GetValue<T>(key);

    public IEnumerable<T> GetValues<T>(string key) =>
        entries.GetValues<T>(key);

    public bool TryGetValue<T>(string key, out T value) =>
        entries.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}

