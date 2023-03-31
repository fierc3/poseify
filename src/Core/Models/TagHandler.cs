using Raven.Client.Documents;
using System;
using System.IO;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class TagHandler : ITagHandler
{
    private readonly ILogger<TagHandler> _logger;
    private readonly IDocumentStore _store;

    public TagHandler(ILogger<TagHandler> logger)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
    }

    public Tag CreateTag(string name)
    {
        string guid = Guid.NewGuid().ToString();
        Tag? tag = null;
        using (var session = _store.OpenSession())
        {
            tag = new()
            {
                InternalGuid = guid,
                DisplayName = name,
            };
            session.Store(tag, guid);
            session.SaveChanges();
            tag = session.Query<Tag>().Where(x => x.InternalGuid == guid).FirstOrDefault();
        }
        return tag;
    }

    public IEnumerable<Tag> GetTagsFromGuid(IEnumerable<string> tagGuids)
    {
        IEnumerable<Tag>? tags = null;
        using (var session = _store.OpenSession())
        {
            tags = session.Query<Tag>().Where(x => tagGuids.Contains(x.InternalGuid));
        }
        return tags;
    }

    public Tag GetTagsFromGuid(string tagGuid)
    {
        Tag? tag = null;
        using (var session = _store.OpenSession())
        {
            tag = session.Query<Tag>().Where(x => x.InternalGuid == tagGuid).FirstOrDefault();
        }
        return tag;
    }
}

