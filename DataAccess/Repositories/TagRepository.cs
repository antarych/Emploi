using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataAccess.NHibernate;
using Journalist;
using NHibernate.Linq;

namespace DataAccess.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ISessionProvider _sessionProvider;

        public TagRepository(ISessionProvider sessionProvider)
        {
            Require.NotNull(sessionProvider, nameof(sessionProvider));
            _sessionProvider = sessionProvider;
        }

        public IList<Tag> CreateTags(IList<string> tags)
        {
            Require.NotNull(tags, nameof(tags));
            IList<Tag> tagList = new List<Tag>();
            foreach (var tag in tags)
            {
                var newTag = CreateTag(tag.ToLower());
                tagList.Add(newTag);
            }
            return tagList;
        }

        public Tag CreateTag(string tag)
        {
            Require.NotNull(tag, nameof(tag));

            var session = _sessionProvider.GetCurrentSession();
            var newTag = new Tag(tag.ToLower());
            var savedTagId = (int)session.Save(newTag);
            return session.Get<Tag>(savedTagId);
        }

        public IList<Tag> GetTags(Func<Tag, bool> predicate = null)
        {
            var session = _sessionProvider.GetCurrentSession();
            return predicate == null
                ? session.Query<Tag>().ToList()
                : session.Query<Tag>().Where(predicate).ToList();
        }

        public void UpdateTag(Tag tag)
        {
            Require.NotNull(tag, nameof(tag));

            var session = _sessionProvider.GetCurrentSession();
            session.Update(tag.TagName.ToLower());
        }
    }
}
