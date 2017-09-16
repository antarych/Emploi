using System;
using System.Collections.Generic;
using Common;

namespace TagManagement.Application
{
    public interface ITagManager 
    {
        Tag CreateTag(string stringTag);

        bool TryFindTag(string stringTag, out int tagId);

        IEnumerable<Tag> GetTag(Func<Tag, bool> predicate = null);

        IEnumerable<Tag> GetOfferedTags(string subTag);

        IEnumerable<string> GetPopularTags();
    }
}
