using System;
using System.Collections.Generic;

namespace Common
{
    public interface ITagRepository
    {
        IList<Tag> GetTags(Func<Tag, bool> predicate = null);

        Tag CreateTag(string tag);

        void UpdateTag(Tag tag);
    }
}
