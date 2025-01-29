namespace AT9.Services
{
    public interface ILayoutNotifier
    {
       

        void RegisterRefresh(Action refreshAction);
        void Refresh();
    }

}