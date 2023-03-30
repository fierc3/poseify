﻿using Raven.Client.Documents;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class TagHandler
{
    private readonly ILogger<TagHandler> _logger;
    private readonly IDocumentStore _store;

    public TagHandler(ILogger<TagHandler> logger)
    {
        _logger = logger;
        _store = DocumentStoreHolder.Store;
    }

    public Tag CreateTag(string name) {
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
}

