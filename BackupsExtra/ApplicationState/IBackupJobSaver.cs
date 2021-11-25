namespace BackupsExtra.ApplicationState
{
    public interface IBackupJobSaver
    {
        void Save(ApplicationContext context);
        ApplicationContext Load();
    }
}