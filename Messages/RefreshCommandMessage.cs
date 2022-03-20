namespace ImageTool.Messages
{
    public class RefreshCommandMessage
    {
        private readonly bool refresh;

        public RefreshCommandMessage(bool input)
        {
            refresh = input;
        }

        public bool Refresh
        {
            get => refresh;
        }
    }
}