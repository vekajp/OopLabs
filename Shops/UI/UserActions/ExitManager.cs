namespace Shops.UI.UserActions
{
    public class ExitManager : Action
    {
        public ExitManager()
            : base("Exit", null)
        {
        }

        public override int Execute()
        {
            return 1;
        }
    }
}