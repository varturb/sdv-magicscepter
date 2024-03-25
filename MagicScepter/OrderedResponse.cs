using StardewValley;

namespace MagicScepter
{
  public class OrderedResponse
  {
    public Response Response { get; set; }
    public int Order { get; set; }

    public OrderedResponse(int order, Response response)
    {
      Order = order;
      Response = response;
    }
  }
}
