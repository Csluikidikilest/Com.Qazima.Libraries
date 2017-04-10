namespace Com.Qazima.Libraries.Server.Http.Actions.Events
{
  public class JsonActionPostEventArgs<ObjectType> : ActionGetEventArgs
  {
    public ObjectType New { get; set; }
  }
}
