using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using TagManagement.Application;

namespace TagManagement.Domain
{
    public class TagManager : ITagManager
    {
        public TagManager(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        private readonly ITagRepository _tagRepository;

        public bool TryFindTag(string stringTag, out int tagId)
        {
            var existingTag = _tagRepository.GetTags(tag => tag.TagName == stringTag).SingleOrDefault();
            tagId = existingTag?.TagId ?? 0;
            return existingTag != null;
        }

        public Tag CreateTag(string stringTag)
        {
            var tag =_tagRepository.CreateTag(stringTag);
            return tag;
        }

        public IEnumerable<Tag> GetTag(Func<Tag, bool> predicate = null)
        {
            return _tagRepository.GetTags(predicate);
        }

        public IEnumerable<Tag> GetOfferedTags(string subTag)
        {
            return _tagRepository.GetTags(t => t.TagName.StartsWith(subTag.ToLower()));
        }

        public IEnumerable<string> GetPopularTags()
        {
            var popTags = new List<string>();
            popTags.Add("Всякая");
            popTags.Add("фигня");
            popTags.Add("чтобы");
            popTags.Add("у тебя");
            popTags.Add("работало");
            return popTags;
        }
    }
}
