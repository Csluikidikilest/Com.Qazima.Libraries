namespace Com.Qazima.Libraries.Server.Http
{
  public interface IHasId
  {
    int Id { get; set; }
  }

  public interface IHasId<Type> : IHasId
  {
    new Type Id { get; set; }
  }
}
