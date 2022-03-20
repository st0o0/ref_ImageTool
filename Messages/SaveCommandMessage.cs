namespace ImageTool.Messages
{
    public class SaveCommandMessage
    {
        private readonly bool _save;

        public SaveCommandMessage(bool var)
        {
            this._save = var;
        }

        public bool Save
        {
            get => _save;
        }
    }
}