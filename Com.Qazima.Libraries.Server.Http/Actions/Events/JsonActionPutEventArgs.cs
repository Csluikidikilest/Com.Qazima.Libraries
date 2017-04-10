namespace Com.Qazima.Libraries.Server.Http.Actions.Events
{
  public class JsonActionPutEventArgs<ObjectType> : ActionGetEventArgs
  {
    public ObjectType New { get; set; }

    public ObjectType Old { get; set; }
  }
}
