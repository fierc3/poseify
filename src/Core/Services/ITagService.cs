
public interface ITagService
{
    public Tag CreateTag(string name);
    public IEnumerable<Tag> GetTagsFromGuid(IEnumerable<string> tagGuids);
    public Tag GetTagsFromGuid(string tagGuid);

}

