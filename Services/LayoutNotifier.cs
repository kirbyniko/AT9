namespace AT9.Services
{
    public class LayoutNotifierService : ILayoutNotifier
    {
        private Action? _refreshAction;
        public void RegisterRefresh(Action refreshAction)
        {
            _refreshAction = refreshAction;
        }

        public void Refresh()
        {
            _refreshAction?.Invoke();
        }
    }


}
