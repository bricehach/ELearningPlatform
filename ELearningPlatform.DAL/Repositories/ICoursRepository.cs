using ELearningPlatform.DAL.Models;


namespace ELearningPlatform.Repositories
{
    public interface ICoursRepository : IRepository<Cours>
    {
        // Ajoute ici des méthodes spécifiques aux cours si nécessaire
    }

    public interface IRepository<T>
    {
    }
}