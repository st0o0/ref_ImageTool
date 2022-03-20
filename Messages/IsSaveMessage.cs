namespace ImageTool.Messages
{
    public class IsSaveMessage
    {
        private readonly bool _isSaved;

        public IsSaveMessage(bool IsSave)
        {
            _isSaved = IsSave;
        }

        public bool IsSaved
        {
            get => _isSaved;
        }
    }
}