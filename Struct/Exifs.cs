namespace ImageTool.Struct
{
    public struct Exifs
    {
        public Exifs(string tagName, string tagdescription)
        {
            TagName = tagName;
            TagDescription = tagdescription;
        }

        public string TagName { get; }
        public string TagDescription { get; }
    }
}