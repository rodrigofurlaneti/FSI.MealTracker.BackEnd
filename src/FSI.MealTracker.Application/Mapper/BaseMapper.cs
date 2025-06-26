namespace FSI.MealTracker.Application.Mapper
{
    public abstract class BaseMapper<TDto, TEntity>
    {
        public abstract TDto ToDto(TEntity entity);
        public abstract TEntity ToEntity(TDto dto);
    }
}
