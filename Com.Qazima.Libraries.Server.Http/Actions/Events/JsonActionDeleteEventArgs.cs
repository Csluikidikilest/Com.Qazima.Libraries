namespace Com.Qazima.Libraries.Server.Http.Actions.Events
{
  public class JsonActionDeleteEventArgs<ObjectType> : ActionGetEventArgs
  {
    public ObjectType Old { get; set; }
  }
}
