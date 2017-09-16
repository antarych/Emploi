namespace Common
{
    public class Tag
    {
        public Tag(string tagName)
        {
            TagName = tagName;
        }

        protected Tag()
        {
            
        }

        public virtual int TagId { get; set; }

        public virtual string TagName { get; set; }
    }
}
