namespace Mobi.Repository
{
    public interface IRepository<TEntity> where TEntity : Data.BaseEntity
    {

        IList<TEntity> GetAllList();
        IEnumerable<TEntity> GetAll();
        void Insert(TEntity entity);

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TEntity GetById(object id);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(TEntity entity);

        #endregion

    }
}
