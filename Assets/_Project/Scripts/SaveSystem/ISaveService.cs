namespace _Project.Scripts.SaveSystem
{
    public interface ISaveService
    {
        void Save();
        void Save(SaveData saveData);
        SaveData Load();
        void DeleteAllSaves();
    }
}