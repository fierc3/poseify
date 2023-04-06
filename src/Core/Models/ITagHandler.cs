
public interface ITagHandler
{
    public Tag CreateTag(string name);
    public IEnumerable<Tag> GetTagsFromGuid(IEnumerable<string> tagGuids);
    public Tag GetTagsFromGuid(string tagGuid);

}

