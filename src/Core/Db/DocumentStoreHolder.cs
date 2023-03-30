using Raven.Client.Documents;

public class DocumentStoreHolder
{
    private readonly static Lazy<IDocumentStore> _store =
        new Lazy<IDocumentStore>(CreateDocumentStore);

    private static IDocumentStore CreateDocumentStore()
    {
        var store = new DocumentStore
        {
            Urls = new[] { "http://localhost:6969" },
            Database = "Dev"
        };

        store.Initialize();
        return store;
    }

    public static IDocumentStore Store
    {
        get { return _store.Value; }
    }
}