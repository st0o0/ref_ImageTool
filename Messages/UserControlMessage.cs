using System.Windows.Controls;

namespace ImageTool.Messages
{
    public class UserControlMessage
    {
        private readonly UserControl userControl;

        public UserControlMessage(UserControl usercontrol)
        {
            userControl = usercontrol;
        }

        public UserControl GetUserControl
        {
            get => userControl;
        }
    }
}